using LogicLibrary;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GI.Wins
{
    /// <summary>
    /// Логика взаимодействия для Pers.xaml
    /// </summary>
    public partial class Pers : Window
    {
        int currentBackgroundIndex = 0;
        readonly Uri[] backgrounds = new Uri[5];
        public Pers(BrawlersDocument pers)
        {
            InitializeComponent();

            GridDate.DataContext = pers;

            //Заполняем массив изображений
            for (int i = 1; i <= backgrounds.Length; i++)
            {
                backgrounds[i - 1] = new Uri($"pack://application:,,,/imgs/BG/bgVideo{i}.mp4");
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            // Изменяем текущий индекс фона
            currentBackgroundIndex++;
            if (currentBackgroundIndex >= backgrounds.Length)
            {
                currentBackgroundIndex = 0;
            }

            // Присваиваем новое фоновое изображение PictureBox
            //BackMedia.Source = null;
            BackMedia.Source = backgrounds[currentBackgroundIndex];
            // Получение ссылки на объект MediaElement

            MediaElement mediaElement = BackMedia;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
