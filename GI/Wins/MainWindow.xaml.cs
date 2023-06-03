using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using GI.Wins;
using LogicLibrary;
using MaterialDesignColors.Recommended;
using static GI.MainWindow;
using Image = System.Windows.Controls.Image;

namespace GI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
	{
		readonly BrawlersManager charactersManager = new BrawlersManager();
        readonly ImageManager imageManager = new ImageManager();
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
        readonly ImageBrush[] backgrounds = new ImageBrush[12];
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
            for (int i = 1; i <= 12; i++)
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
            //await imageManager.UploadImageAsync();
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
			//// Применение фильтра к данным Characters на основе SearchText
			//var filteredCharacters = charactersActual.Where(c => c.Name.Contains(((TextBox)sender).Text)).ToList();
			//if (filteredCharacters.Count == 0)
			//	listBox.Width = listBox.ActualWidth;
			//else
			//	listBox.Width = double.NaN;
			//// Обновление отображаемых данных в ListBox
			//listBox.ItemsSource = filteredCharacters;
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

		private void Image_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e) // Добавление/Удаление в/из Избранного
		{
   //         Image clickedImage = (Image)sender;
   //         ListBoxItem listBoxItem = FindParent<ListBoxItem>(clickedImage); // Получаем родительский listBoxItem

   //         if (listBoxItem != null)
			//{
   //             // Получаем DataContext выбранного элемента
   //             if (listBoxItem.DataContext is CharacterDocument selectedCharacter)
   //             {
   //                 if (user.FavoriteСharacters != null && user.FavoriteСharacters.Contains(selectedCharacter.Name)) // Если уже в избранном
   //                 {
   //                     user.FavoriteСharacters.Remove(selectedCharacter.Name);
   //                     ((System.Windows.Controls.Image)sender).Source = imageSourceFavEmpty;

   //                     CharacterDocument characterToRemove = charactersFav.FirstOrDefault(c => c.Name == selectedCharacter.Name);
   //                     int character1 = charactersPl.FindIndex(c => c.Name == selectedCharacter.Name);
   //                     int character2 = charactersNoPl.FindIndex(c => c.Name == selectedCharacter.Name);
   //                     if (characterToRemove != null)
   //                     {
   //                         charactersFav.Remove(characterToRemove);
   //                         if (character1 != -1)
   //                             charactersPl[character1].FavoriteСharacters = pathFavEmpty;
   //                         if (character2 != -1)
   //                             charactersNoPl[character2].FavoriteСharacters = pathFavEmpty;

   //                         listBox3.ItemsSource = null;
   //                         listBox3.ItemsSource = charactersFav;
   //                         listBox2.ItemsSource = null;
   //                         listBox2.ItemsSource = charactersNoPl;
   //                         listBox1.ItemsSource = null;
   //                         listBox1.ItemsSource = charactersPl;
   //                     }
   //                 }

   //                 else // Если не в избранном
   //                 {
   //                     user.FavoriteСharacters.Add(selectedCharacter.Name);
   //                     ((System.Windows.Controls.Image)sender).Source = imageSourceFav;

   //                     CharacterDocument characterToAdd = characters.FirstOrDefault(c => c.Name == selectedCharacter.Name);
   //                     int character1 = charactersPl.FindIndex(c => c.Name == selectedCharacter.Name);
   //                     int character2 = charactersNoPl.FindIndex(c => c.Name == selectedCharacter.Name);
   //                     if (characterToAdd != null)
   //                     {
   //                         charactersFav.Add(characterToAdd);
   //                         if (character1 != -1)
   //                             charactersPl[character1].FavoriteСharacters = pathFav;
   //                         if (character2 != -1)
   //                             charactersNoPl[character2].FavoriteСharacters = pathFav;
   //                         charactersActual = charactersFav;

   //                         listBox3.ItemsSource = null;
   //                         listBox3.ItemsSource = charactersFav;
   //                         listBox2.ItemsSource = null;
   //                         listBox2.ItemsSource = charactersNoPl;
   //                         listBox1.ItemsSource = null;
   //                         listBox1.ItemsSource = charactersPl;
   //                     }
   //                 }
   //             }
   //         }
        }

        //private T FindParent<T>(DependencyObject child) where T : DependencyObject // Поиск родителя
        //{
        //    //DependencyObject parent = VisualTreeHelper.GetParent(child);

        //    //if (parent == null)
        //    //    return null;

        //    //T parentT = parent as T;
        //    //return parentT ?? FindParent<T>(parent);
        //}

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

                        //case "По оружию":
                        //    charactersPl = charactersPl.OrderByDescending(c => c.Weapon).ToList();
                        //    charactersNoPl = charactersNoPl.OrderByDescending(c => c.Weapon).ToList();
                        //    charactersFav = charactersFav.OrderByDescending(c => c.Weapon).ToList();
                        //    break;
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
            var brs = new BrawlersDocument() { Stats = new Stats()};
            brs.Name = nameTextBox.Text;
            brs.Class = classTextBox.Text;
            brs.Description = descriptionTextBox.Text;
            brs.Rarity = rarityTextBox.Text;
            brs.Stats.Health = Convert.ToInt32(healthTextBox.Text);
            brs.Stats.AttackName = attackNameTextBox.Text;
            brs.Stats.AttackDescription = attackDescriptionTextBox.Text;
            brs.Stats.AttackValue = attackValueTextBox.Text;
            brs.Stats.AttackDistance = attackDistanceTextBox.Text;
            brs.Stats.AttackSpeed = attackSpeedTextBox.Text;
            brs.Stats.AttackAdditionalItemName = attackAdditionalItemNameTextBox.Text;
            brs.Stats.AttackAdditionalItemValue = attackAdditionalItemValueTextBox.Text;
            brs.Stats.SuperAttackName = superAttackNameTextBox.Text;
            brs.Stats.SuperAttackDescription = superAttackDescriptionTextBox.Text;
            brs.Stats.SuperAttackValue = superAttackValueTextBox.Text;
            brs.Stats.SuperAttackDistance = superAttackDistanceTextBox.Text;
            brs.Stats.SuperAttackAdditionalItemName = superAttackAdditionalItemNameTextBox.Text;
            brs.Stats.SuperAttackAdditionalItemValue = superAttackAdditionalItemValueTextBox.Text;
            brs.Stats.CompanionHealth = CompanionHealthTextBox.Text;
            brs.Stats.CompanionAttack = CompanionAttackTextBox.Text;
            brs.Stats.CompanionSpeed = CompanionSpeedTextBox.Text;
            brs.Stats.CompanionDistance = CompanionDistanceTextBox.Text;
            brs.Stats.Speed = CompanionSpeedTextBox.Text;
            brs.Stats.Feature = featureTextBox.Text;

            brs.Category = "Играбельный";
            brs.UploadDate = DateTime.Now;

            await brawlersManager.UploadCharacterAsync(brs);
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
    }
}
