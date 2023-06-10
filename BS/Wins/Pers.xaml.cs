using BS;
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
        readonly BrawlersDocument pers;
        public Pers(BrawlersDocument pers)
        {
            InitializeComponent();

            this.pers = pers;
            GridDate.DataContext = pers;
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Keyboard.Focus(this);
            }));
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PersInfo persInfo = new PersInfo(pers);
            persInfo.Show();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }
}
