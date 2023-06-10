using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LogicLibrary
{
    // Коллекция изображений
    public class ImageManager
    {
        private readonly string _connectionString;
        private readonly string _databaseName;
        private readonly string _collectionName;
        private readonly string _directoryPath;

        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<ImageDocument> _collection;

        public ImageManager(string databaseName = "DB", string collectionName = "BrawlersImage", string directoryPath = null, 
            string connectionString = "mongodb://localhost:27017", string directory = "pers")
        {
            _connectionString = connectionString;
            _databaseName = databaseName;
            _collectionName = collectionName;

            if (string.IsNullOrEmpty(directoryPath))
            {
                directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BS", directory);
            }

            Directory.CreateDirectory(directoryPath);
            _directoryPath = directoryPath;

            _client = new MongoClient(_connectionString);
            _database = _client.GetDatabase(_databaseName);
            _collection = _database.GetCollection<ImageDocument>(_collectionName);
        }

        public async Task<bool> UploadImageAsync(string imgPath, string filename) // Загрузка в БД конкретное изображение
        {
            if (imgPath != null)
            {
                filename = Path.ChangeExtension(filename, ".png");
                var file = new FileInfo(Path.Combine(imgPath, filename));

                var imageBytes = await Task.Run(() => File.ReadAllBytes(imgPath));
                var imageDocument = new ImageDocument(filename, imageBytes);
                await _collection.InsertOneAsync(imageDocument);

                return file.Exists;
            }

            return false;
        }

        public async Task UploadImageAsync() // Загрузка в БД все изображения из папки (Не для пользовательского пользования, поэтому не нужна обработка исключения)
        {
            var directory = new DirectoryInfo(_directoryPath);

            foreach (var file in directory.GetFiles())
            {
                var imageBytes = await Task.Run(() => File.ReadAllBytes(file.FullName));

                var document = new ImageDocument(file.Name, imageBytes);

                await _collection.InsertOneAsync(document);
            }
        }

        public async Task<bool> LoadImageFromDbAsync(string filename) // Загрузка из БД конкретное изображение
        {
            filename = Path.ChangeExtension(filename, Path.GetExtension(filename));

            var filter = Builders<ImageDocument>.Filter.Eq("filename", filename);
            var document = await _collection.Find(filter).FirstOrDefaultAsync();

            if (document != null)
            {
                var bytes = document.ImageBytes;
                await Task.Run(() => File.WriteAllBytes(Path.Combine(_directoryPath, filename), bytes));
            }

            return document != null;
        }

        public async Task LoadImageFromDbAsync() // Загрузка из БД всех изображений 
        {
            var filter = Builders<ImageDocument>.Filter.Empty;
            var documents = await _collection.Find(filter).ToListAsync();

            var tasks = documents.Select(document => ProcessImageDocumentAsync(document)).ToList();

            await Task.WhenAll(tasks);
        }

        private async Task ProcessImageDocumentAsync(ImageDocument document) // Загрузка из БД отдельного изображения через метод загрузки всех
        {
            var filename = document.Filename;
            var path = Path.Combine(_directoryPath, filename);

            // Проверяем, существует ли файл в папке
            if (!File.Exists(path))
            {
                var bytes = document.ImageBytes;
                await Task.Run(() => File.WriteAllBytes(path, bytes));
            }
        }

        public async Task<bool> DeleteImageAsync(string filename) // Удаление из БД изображения коллекции
        {
            var filter = Builders<ImageDocument>.Filter.Eq(c => c.Filename, filename + ".png");
            var result = await _collection.DeleteOneAsync(filter);

            return result.DeletedCount != 0;
        }
    }
}
