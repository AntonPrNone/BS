using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using GI.Wins;
using LogicLibrary;
using Microsoft.Win32;
using Path = System.IO.Path;

namespace GI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
	{
        readonly BrawlersManager brawlersManager = new BrawlersManager();
        readonly ImageManager imageManager = new ImageManager();
        readonly ImageManager imageIcoManager = new ImageManager(directory: "persIco", collectionName: "BrawlersImageIco");
        readonly ImageManager imageConceptManager = new ImageManager(collectionName: "BrawlersImageConcept");
        readonly ImageManager imageIcoConceptManager = new ImageManager(directory: "persIco", collectionName: "BrawlersImageIcoConcept");

        readonly string directoryPathIco = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BS", "persIco");
        readonly string directoryPathImg = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BS", "pers");
        string imgPath, imgIcoPath;
        readonly string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LogPas.txt");
        readonly List<string> deletePath = new List<string>();

        readonly string username;
		bool initial = true;
		User user;
        int currentBackgroundIndex = 0;
        readonly ImageBrush[] backgrounds = new ImageBrush[11];
        List<BrawlersDocument> brawlers;
        List<BrawlersDocument> conceptBrawlers;
        List<BrawlersDocument> brawlersCopy = new List<BrawlersDocument>();
        public MainWindow()
		{
			InitializeComponent();
            ItemsContolList.Opacity = 0;
			username = File.ReadAllLines(path)[0];

			LoadData();

            //Заполняем массив изображений
            for (int i = 1; i <= backgrounds.Length; i++)
            {
                backgrounds[i - 1] = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/imgs/BG/bg{i}.jpg")));
            }
        }

        private async Task UpdateData() // Импорт и расставление данных
		{
            user = await UserManager.GetUserAsync(username);
            if (user.ConceptBrawlers == null) user.ConceptBrawlers = new List<string>();
            Username_TextBlock.Text = username;
            brawlers = await brawlersManager.LoadCharacterAsync();
            conceptBrawlers = await brawlersManager.LoadCharacterAsync(user.ConceptBrawlers);
            await imageManager.LoadImageFromDbAsync();
            await imageIcoManager.LoadImageFromDbAsync();

            await imageConceptManager.LoadImageFromDbAsync();
            await imageIcoConceptManager.LoadImageFromDbAsync();

            foreach (BrawlersDocument brawlersDocument in brawlers)
            {
                if (brawlersDocument.Name != null)
                {
                    brawlersDocument.PhotoIco = Path.Combine(directoryPathIco, Path.ChangeExtension(brawlersDocument.Name, ".png"));
                    brawlersDocument.Photo = Path.Combine(directoryPathImg, Path.ChangeExtension(brawlersDocument.Name, ".png"));
                }
            }

            foreach (BrawlersDocument brawlersDocument in conceptBrawlers)
            {
                if (brawlersDocument.Name != null)
                {
                    brawlersDocument.PhotoIco = Path.Combine(directoryPathIco, Path.ChangeExtension(brawlersDocument.Name, ".png"));
                    brawlersDocument.Photo = Path.Combine(directoryPathImg, Path.ChangeExtension(brawlersDocument.Name, ".png"));
                }
            }

            ItemsContolList.ItemsSource = brawlers;
            ItemsContolList2.ItemsSource = conceptBrawlers;
            brawlersCopy.AddRange(brawlers);
        }

		private async void LoadData() // Загрузка данных с анимацией загрузки
		{
            _ = new Task[]
            {
                Anim.FadeInAsync(LoadingCircle, 1.5),
                Anim.FadeInAsync(Ldg_TextBlock, 1.5)
            };

            await UpdateData();
            await Task.Delay(1);

            _ = new Task[]
            {
                Anim.FadeOutAsync(LoadingCircle, 1.5),
                Anim.FadeOutAsync(Ldg_TextBlock, 1.5)
            };
        }

		private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) // Поиск персонажа
		{
            // Применение фильтра к данным Characters на основе SearchText
            var filteredCharacters = brawlers.Where(c => c.Name.ToLower().Contains(((TextBox)sender).Text.ToLower())).ToList();
            if (filteredCharacters.Count == 0)
                ItemsContolList.Width = ItemsContolList.ActualWidth;
            else
                ItemsContolList.Width = double.NaN;
            // Обновление отображаемых данных в ListBox
            ItemsContolList.ItemsSource = filteredCharacters;
        }

        private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) // Выбор критерия сортировки в ComboBox
        {
            if (!initial)
            {
                ComboBox comboBox = (ComboBox)sender;
                ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;
                string selectedSorting = selectedItem.Content.ToString();

                // Выполнение сортировки в зависимости от выбранного варианта
                switch (selectedSorting)
                {
                    case "По имени ↑":
                        brawlersCopy = brawlersCopy.OrderBy(c => c.Name).ToList();
                        break;

                    case "По имени ↓":
                        brawlersCopy = brawlersCopy.OrderByDescending(c => c.Name).ToList();
                        break;

                    case "По редкости ↑":
                        brawlersCopy = brawlersCopy.OrderBy(c => c.RarityPriority).ToList();
                        break;

                    case "По редкости ↓":
                        brawlersCopy = brawlersCopy.OrderByDescending(c => c.RarityPriority).ToList();
                        break;

                    case "По классу":
                        brawlersCopy = brawlersCopy.OrderByDescending(c => c.Class).ToList();
                        break;
                }

                // Обновление источника данных для ListBox
                await Anim.FadeOutAsync(ItemsContolList, 0.5);

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    ItemsContolList.ItemsSource = brawlersCopy;
                });

                await Task.Delay(100); // Дополнительная задержка для убедительности

                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await Anim.FadeInAsync(ItemsContolList, 0.5);
                });
            }

            initial = false;
        }

        // Добавление персонажа
        private async void AddButton_ClickAsync(object sender, RoutedEventArgs e)
		{
            var brs = new BrawlersDocument
            {
                Stats = new Stats(),
                Name = FormattingValue(nameTextBox),
                Class = FormattingValue(classTextBox),
                Description = FormattingValue(descriptionTextBox),
                Rarity = FormattingValue(rarityTextBox)
            };

            if (HealthTextBox.Text != "") brs.Stats.Health = Convert.ToInt32(FormattingValue(HealthTextBox));
            brs.Stats.AttackName = FormattingValue(attackNameTextBox);
            brs.Stats.AttackDescription = FormattingValue(attackDescriptionTextBox);
            brs.Stats.AttackValue = FormattingValue(attackValueTextBox);
            brs.Stats.AttackDistance = FormattingValue(attackDistanceTextBox);
            brs.Stats.AttackSpeed = FormattingValue(attackSpeedTextBox);
            brs.Stats.AttackAdditionalItemName = FormattingValue(attackAdditionalItemNameTextBox);
            brs.Stats.AttackAdditionalItemValue = FormattingValue(attackAdditionalItemValueTextBox);
            brs.Stats.SuperAttackName = FormattingValue(superAttackNameTextBox);
            brs.Stats.SuperAttackDescription = FormattingValue(superAttackDescriptionTextBox);
            brs.Stats.SuperAttackValue = FormattingValue(superAttackValueTextBox);
            brs.Stats.SuperAttackDistance = FormattingValue(superAttackDistanceTextBox);
            brs.Stats.SuperAttackAdditionalItemName = FormattingValue(superAttackAdditionalItemNameTextBox);
            brs.Stats.SuperAttackAdditionalItemValue = FormattingValue(superAttackAdditionalItemValueTextBox);
            brs.Stats.CompanionHealth = FormattingValue(CompanionHealthTextBox);
            brs.Stats.CompanionAttack = FormattingValue(CompanionAttackTextBox);
            brs.Stats.CompanionSpeed = FormattingValue(CompanionSpeedTextBox);
            brs.Stats.CompanionDistance = FormattingValue(CompanionDistanceTextBox);
            brs.Stats.Speed = FormattingValue(SpeedTextBox);
            brs.Stats.Feature = FormattingValue(featureTextBox);

            brs.Category = "Концепт";
            brs.UploadDate = DateTime.Now;

            if (await brawlersManager.UploadCharacterAsync(brs))
            {
                user.ConceptBrawlers.Add(brs.Name);
                conceptBrawlers.Add(brs);
            }

            await imageConceptManager.UploadImageAsync(imgPath, brs.Name);
            await imageIcoConceptManager.UploadImageAsync(imgIcoPath, brs.Name);

            brs.Photo = imgPath;
            brs.PhotoIco = imgIcoPath;
            ItemsContolList2.ItemsSource = null;
            ItemsContolList2.ItemsSource = conceptBrawlers;

            user.ConceptBrawlers = conceptBrawlers.Select(b => b.Name).ToList();
            await UserManager.ReplaceUserAsync(user);
            await brawlersManager.UploadCharacterAsync(brs);
            Button_Click(null, e);
        }

        // Форматирование ввода
        private string FormattingValue(TextBox textBox) => Regex.Replace(textBox.Text.Replace("\r", " ").Replace("\n", " "), @"\s+", " ").Trim();


        private void Button_Click(object sender, RoutedEventArgs e) // Очищение полей
        {
            AddGrid.DataContext = new BrawlersDocument() { Stats = new Stats() };
            HealthTextBox.Text = "";
            ImagePathTextBox.Text = "";
            ImageIcoPathTextBox.Text = "";
        }

        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) // Смена фона
        {
            TabContolBG.ImageSource = null;
            // Изменяем текущий индекс фона
            currentBackgroundIndex++;
            if (currentBackgroundIndex >= backgrounds.Length)
            {
                currentBackgroundIndex = 0;
            }

            // Присваиваем новое фоновое изображение PictureBox
            TabControl.Background = backgrounds[currentBackgroundIndex];
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) // Открытие окна о персонаже
        {
            if (sender is Grid grid)
            {
                if (grid.FindName("NameBinding_TextBlock") is TextBlock textBlock)
                {
                    BrawlersDocument brawler = brawlers.FirstOrDefault(x => x.Name == textBlock.Text) ?? conceptBrawlers.FirstOrDefault(x => x.Name == textBlock.Text);
                    Pers pers = new Pers(brawler);
                    pers.Show();
                }
            }
        }

        private async void Image_MouseRightButtonDown(object sender, MouseButtonEventArgs e) // Удаление концепт-персонажа
        {
            // Находим родительский элемент Grid 
            var element = sender as UIElement;
            while (element != null && !(element is Grid))
            {
                element = VisualTreeHelper.GetParent(element) as UIElement;
            }

            var grid = element as Grid;
            // Ищем в дочерних элементах TextBlock с именем NameBinding_TextBlock
            var img = grid.FindName("pers") as Image;
            // Если элемент найден, выводим его содержимое
            if (grid.FindName("NameBinding_TextBlock") is TextBlock textBlock)
            {
                BrawlersDocument brawler = brawlers.FirstOrDefault(x => x.Name == textBlock.Text) ?? conceptBrawlers.FirstOrDefault(x => x.Name == textBlock.Text);
                img.Source = null;
                conceptBrawlers.Remove(brawler);
                user.ConceptBrawlers.Remove(brawler.Name);
                await brawlersManager.DeleteCharacterAsync(brawler.Name);
                await imageConceptManager.DeleteImageAsync(brawler.Name);
                await imageIcoConceptManager.DeleteImageAsync(brawler.Name);
                await UserManager.ReplaceUserAsync(user);

                conceptBrawlers.Remove(conceptBrawlers.FirstOrDefault(b => b.Name == brawler.Name));

                ItemsContolList2.Resources.Clear();
                ItemsContolList2.ItemsSource = null;
                ItemsContolList2.ItemsSource = conceptBrawlers;

                string path1 = Path.Combine(directoryPathImg, brawler.Name + ".png");
                string path2 = Path.Combine(directoryPathIco, brawler.Name + ".png");

                deletePath.Add(path1);
                deletePath.Add(path2);
            }
        }

        private void NameTextBox_LostFocus(object sender, RoutedEventArgs e) // Проверка на наличие персонажа с данным именем и заполенение его данными формы в случае истины
        {
            BrawlersDocument brawler = conceptBrawlers.FirstOrDefault(x => x.Name == nameTextBox.Text);
            if (brawler != null) AddGrid.DataContext = brawler;

        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) // При смене вкладки
        {
            if (TabControl.SelectedIndex != 1)
            {
                foreach (var path in deletePath)
                {
                    try
                    {
                        File.Delete(path);
                    }

                    catch (Exception)
                    {
                        
                    }
                }
            }
        }

        private void HealthTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e) // Проверка численного ввода в HealthTextBox
        {
            // Проверка, является ли введенный символ числом
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                // Предотвращение ввода символа, если это не число
                e.Handled = true;
            }

            // Проверка, является ли введенный символ пробелом или пустым символом
            else if (char.IsWhiteSpace(e.Text, e.Text.Length - 1) || string.IsNullOrWhiteSpace(e.Text))
            {
                // Предотвращение ввода пробела или пустого символа
                e.Handled = true;
            }
        }

        private void AddImage_Button_Click(object sender, RoutedEventArgs e) // Выбор изображения
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg;*.gif;*.bpm;*.raw;*.svg)|*.png;*.jpeg;*.jpg;*.gif;*.bpm;*.raw;*.svg";
            if (openFileDialog.ShowDialog() == true)
            {
                imgPath = openFileDialog.FileName;
                ImagePathTextBox.Text = imgPath;
            }
        }

        private void AddImageIco_Button_Click(object sender, RoutedEventArgs e) // Выбор изображения
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg;*.gif;*.bpm;*.raw;*.svg)|*.png;*.jpeg;*.jpg;*.gif;*.bpm;*.raw;*.svg";
            if (openFileDialog.ShowDialog() == true)
            {
                imgIcoPath = openFileDialog.FileName;
                ImageIcoPathTextBox.Text = imgIcoPath;
            }
        }

        // --------------------------------- Взаимодействие с окном --------------------------------------------

        private async void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) // Выход из профиля
        {
            File.WriteAllText(path, string.Empty);
            RegWin regWin = new RegWin();
            regWin.Show();
            await Anim.FadeOut2Async(this);
            Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e) // Свернуть окно
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) // Закрытие окна
        {
            var closingAnimation = (Storyboard)FindResource("ClosingAnimation");
            closingAnimation.Completed += (s, _) => Close();
            BeginStoryboard(closingAnimation);
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) // Перемещение окна
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private async void Window_Closed(object sender, EventArgs e) // Перед закрытием окна
		{
            user.ConceptBrawlers = conceptBrawlers.Select(b => b.Name).ToList();
			await UserManager.ReplaceUserAsync(user);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e) // После отрисовки окна
        {
            await Anim.FadeInAsync(this, 1);
            await Anim.FadeInAsync(ItemsContolList, 1);
        }
    }
}
