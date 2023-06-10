using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogicLibrary
{
    public class BrawlersManager
    {
        private readonly IMongoCollection<BrawlersDocument> _characters;
        private string category;
        public BrawlersManager(string databaseName = "DB", string collectionName = "Brawlers", string connectionString = "mongodb://localhost:27017", string category = "Играбельный")
        {
            this.category = category;
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _characters = database.GetCollection<BrawlersDocument>(collectionName);
        }

        public async Task<bool> UploadCharacterAsync(BrawlersDocument character) // Добавление персонажа в БД
        {
            var filter = Builders<BrawlersDocument>.Filter.Eq(c => c.Name, character.Name);
            var existingCharacter = await _characters.Find(filter).FirstOrDefaultAsync();

            if (existingCharacter == null)
            {
                await _characters.InsertOneAsync(character);
            }

            else
            {
                character.Id = existingCharacter.Id; // сохраняем Id существующего документа в новом документе
                await _characters.ReplaceOneAsync(filter, character);
            }

            return existingCharacter == null;
        }

        public async Task<BrawlersDocument> LoadCharacterAsync(string name) // Получение персонажа из БД
        {
            var filter = Builders<BrawlersDocument>.Filter.Eq(c => c.Name, name);
            return await _characters.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<List<BrawlersDocument>> LoadCharacterAsync() // Получение всех персонажей из БД
        {
            return await _characters.Find(c => c.Category == category).ToListAsync();
        }

        public async Task<List<BrawlersDocument>> LoadCharacterAsync(List<string> listBrawlers) // Получение всех персонажей из БД
        {
            var filter = Builders<BrawlersDocument>.Filter.In(c => c.Name, listBrawlers);
            return await _characters.Find(filter).ToListAsync();
        }

        public async Task<bool?> DeleteCharacterAsync(string name) // Удаление персонажа из БД
        {
            var filter = Builders<BrawlersDocument>.Filter.Eq(c => c.Name, name);
            var result = await _characters.DeleteOneAsync(filter);

            return result.IsAcknowledged && result.DeletedCount > 0;
        }
    }
}
