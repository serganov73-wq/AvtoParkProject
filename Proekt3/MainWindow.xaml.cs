using Proekt3.Вход_и_Регистрация;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Proekt3
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Users CurrentUser { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new Vxod());
        }

        private void Vhod_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Vxod());
        }

        private void Registr_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Registracia());
        }

        private void BtnVehicles_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUser == null)
            {
                MessageBox.Show("Пожалуйста, сначала авторизуйтесь!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; 
            }
            MainFrame.Navigate(new Proekt3.ТранСредства__Водители_и_Рейсы.Vehicles(CurrentUser));
        }

        private void BtnDrivers_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUser == null)
            {
                MessageBox.Show("Пожалуйста, сначала авторизуйтесь!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            MainFrame.Navigate(new Proekt3.ТранСредства__Водители_и_Рейсы.DriversPage(CurrentUser));
        }

        private void BtnRoutes_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUser == null)
            {
                MessageBox.Show("Пожалуйста, сначала авторизуйтесь!", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            MainFrame.Navigate(new Proekt3.ТранСредства__Водители_и_Рейсы.RoutesPage(CurrentUser));
        }
    }
}
