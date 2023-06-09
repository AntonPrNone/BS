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
using HtmlAgilityPack;
using LogicLibrary;
using Microsoft.Win32;

namespace GI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
	{
        // Создание объекта HtmlWeb для загрузки веб-страницы
        HtmlWeb htmlWeb = new HtmlWeb();
        readonly BrawlersManager charactersManager = new BrawlersManager();
        readonly ImageManager imageManager = new ImageManager();
        readonly ImageManager imageIcoManager = new ImageManager(directory: "persIco", collectionName: "BrawlersImageIco");
        readonly string directoryPathIco = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BS", "persIco");
        readonly string directoryPathImg = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BS", "pers");
        readonly string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LogPas.txt");
        readonly string username;
		bool initial = true;
		ListBox listBox;
		User user;
		readonly string pathFavEmpty = @"/imgs/favoriteEmpty.png";
		readonly string pathFav = @"/imgs/favorite.png";
		readonly ImageSource imageSourceFavEmpty = new BitmapImage(new Uri(@"/imgs/favoriteEmpty.png", UriKind.Relative));
		readonly ImageSource imageSourceFav = new BitmapImage(new Uri(@"/imgs/favorite.png", UriKind.Relative));
        int currentBackgroundIndex = 0;
        readonly ImageBrush[] backgrounds = new ImageBrush[11];
        List<BrawlersDocument> brawlers;
        List<BrawlersDocument> brawlersCopy = new List<BrawlersDocument>();
        List<BrawlersDocument> brawlersFav;
        BrawlersManager brawlersManager = new BrawlersManager();
        public MainWindow()
		{
			InitializeComponent();
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
            if (user.FavoriteСharacters == null) user.FavoriteСharacters = new List<string>();
            Username_TextBlock.Text = username;
            brawlers = await brawlersManager.LoadCharacterAsync();
            await imageManager.LoadImageFromDbAsync();
            await imageIcoManager.LoadImageFromDbAsync();

            if (user.FavoriteСharacters != null)
            {
                foreach (string favoriteCharacterName in user.FavoriteСharacters)
                {
                    int chr = brawlers.FindIndex(c => c.Name == favoriteCharacterName);
                    if (chr != -1)
                    {
                        brawlers[chr].FavoriteСharacters = pathFav;
                    }
                }
            }

            foreach (BrawlersDocument brawlersDocument in brawlers)
            {
                if (brawlersDocument.FavoriteСharacters == null || brawlersDocument.FavoriteСharacters == "")
                    brawlersDocument.FavoriteСharacters = pathFavEmpty;
                if (brawlersDocument.Name != null)
                {
                    brawlersDocument.PhotoIco = Path.Combine(directoryPathIco, Path.ChangeExtension(brawlersDocument.Name, ".png"));
                    brawlersDocument.Photo = Path.Combine(directoryPathImg, Path.ChangeExtension(brawlersDocument.Name, ".png"));
                }
            }

            ItemsContolList.ItemsSource = brawlers;

            brawlersFav = (List<BrawlersDocument>)(brawlers.Where(brawler => brawler.FavoriteСharacters == pathFav).ToList());
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

		private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) // Смена вкладки
		{
			//if (!initial) // Если сейчас не инциализация окна
			//{
			//	Search_TextBox.Text = string.Empty;
			//	var selectedTab = TabControl.SelectedIndex;
			//	if (selectedTab == 0)
			//	{
			//		listBox = listBox1;
			//		charactersActual = charactersPl;
			//	}

			//	else if (selectedTab == 1)
			//	{
			//		listBox = listBox2;
			//		charactersActual = charactersNoPl;

			//	}

			//	else if (selectedTab == 2)
			//	{
			//		listBox = listBox3;
			//		charactersActual = charactersFav;
			//	}

			//	listBox.ItemsSource = charactersActual;
			//}

			//initial = false;
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

        // Непользовательское добавление персонажа
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

            if (healthTextBox.Text != "") brs.Stats.Health = Convert.ToInt32(FormattingValue(healthTextBox));
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

            brs.Category = "Играбельный";
            brs.UploadDate = DateTime.Now;

            await brawlersManager.UploadCharacterAsync(brs);
            Button_Click(null, e);
        }

        private void nameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {   
            var brs = new BrawlersDocument();
            // Загрузка веб-страницы
            var htmlDocument = htmlWeb.Load($"https://brawlstars.fandom.com/ru/wiki/{nameTextBox.Text}");

            if (htmlDocument != null)
            {
                // Используя XPath, извлечение нужных элементов из HTML-документа
                nameTextBox.Text = DataExtraction(htmlDocument, "//*[@id=\"firstHeading\"]/span");
                rarityTextBox.Text = DataExtraction(htmlDocument, "//*[@id=\"mw-content-text\"]/div/aside/div[1]/div");
                descriptionTextBox.Text = DataExtraction(htmlDocument, "//*[@id=\"mw-content-text\"]/div/div[1]/i/b");
                classTextBox.Text = DataExtraction(htmlDocument, "/html/body/div[4]/div[3]/div[2]/main/div[3]/div[2]/div/p[2]/i[2]");
                healthTextBox.Text = DataExtraction(htmlDocument, "//*[@id=\"mw-content-text\"]/div/aside/section[1]/section[12]/table/tbody/tr/td[2]");
                attackNameTextBox.Text = DataExtraction(htmlDocument, "//html/body/div[4]/div[3]/div[2]/main/div[3]/div[2]/div/h3[1]/span[2]/b");
                if (attackNameTextBox.Text != "") attackNameTextBox.Text = attackNameTextBox.Text.Substring(7);
                attackDescriptionTextBox.Text = DataExtraction(htmlDocument, "/html/body/div[4]/div[3]/div[2]/main/div[3]/div[2]/div/div[5]/i/b");
                attackValueTextBox.Text = DataExtraction(htmlDocument, "//*[@id=\"mw-content-text\"]/div/aside/section[2]/section[12]/table/tbody/tr/td[2]");
                attackDistanceTextBox.Text = DataExtraction(htmlDocument, "//*[@id=\"mw-content-text\"]/div/aside/section[2]/div[1]/div");
                attackSpeedTextBox.Text = DataExtraction(htmlDocument, "//*[@id=\"mw-content-text\"]/div/aside/section[2]/div[2]/div");
                superAttackNameTextBox.Text = DataExtraction(htmlDocument, "/html/body/div[4]/div[3]/div[2]/main/div[3]/div[2]/div/h3[2]/span[2]/b");
                if (superAttackNameTextBox.Text != "") superAttackNameTextBox.Text = superAttackNameTextBox.Text.Substring(7);
                superAttackDescriptionTextBox.Text = DataExtraction(htmlDocument, "//*[@id=\"mw-content-text\"]/div/div[7]/i/b/text()");
                superAttackValueTextBox.Text = DataExtraction(htmlDocument, "//*[@id=\"mw-content-text\"]/div/aside/section[3]/section[12]/table/tbody/tr/td[2]");
                superAttackDistanceTextBox.Text = DataExtraction(htmlDocument, "/html/body/div[4]/div[3]/div[2]/main/div[3]/div[2]/div/aside/section[3]/div[1]/div");
                featureTextBox.Text = DataExtraction(htmlDocument, "/html/body/div[4]/div[3]/div[2]/main/div[3]/div[2]/div/div[15]/i/b");
                SpeedTextBox.Text = DataExtraction(htmlDocument, "//*[@id=\"mw-content-text\"]/div/aside/div[2]/div/text()[2]");
            }
        }

        static string DataExtraction(HtmlDocument htmlDocument, string xpath)
        {
            var nodes = htmlDocument.DocumentNode.SelectNodes(xpath);

            if (nodes != null)
            {
                // Обработка найденных элементов
                foreach (var node in nodes)
                {
                    // Извлечение нужных данных из элементов
                    var data = node.InnerText;
                    data = Regex.Replace(data.Replace("\r", " ").Replace("\n", " "), @"\s+", " ").Trim(new char[] { '(', ')', '<', '>', '«', '»', ' ' } );
                    // Дальнейшая обработка извлеченных данных
                    return data;
                }
            }

            return "";
        }

        private string FormattingValue(TextBox textBox) => Regex.Replace(textBox.Text.Replace("\r", " ").Replace("\n", " "), @"\s+", " ").Trim();


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddGrid.DataContext = new BrawlersDocument() { Stats = new Stats() };
            healthTextBox.Text = "";
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
			await UserManager.ReplaceUserAsync(user);
		}

        private async void Window_Loaded(object sender, RoutedEventArgs e) // После отрисовки окна
        {
            //await Anim.FadeInAsync(this, 1);
            //await Anim.FadeInAsync(listBox1, 1);
        }

        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Grid grid = sender as Grid;
            if (grid != null)
            {
                TextBlock textBlock = grid.FindName("NameBinding_TextBlock") as TextBlock;
                if (textBlock != null)
                {
                    BrawlersDocument brawler = brawlers.FirstOrDefault(x => x.Name == textBlock.Text);
                    Pers pers = new Pers(brawler);
                    pers.Show();
                }
            }
        }

        private void AddImage_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg;*.gif;*.bpm;*.raw;*.svg)|*.png;*.jpeg;*.jpg;*.gif;*.bpm;*.raw;*.svg";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                ImagePathTextBox.Text = filePath;
            }
        }

        private void AddImageIco_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg;*.gif;*.bpm;*.raw;*.svg)|*.png;*.jpeg;*.jpg;*.gif;*.bpm;*.raw;*.svg";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                ImageIcoPathTextBox.Text = filePath;
            }
        }
    }
}
