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

namespace Proekt3.Вход_и_Регистрация
{
    /// <summary>
    /// Логика взаимодействия для Vxod.xaml
    /// </summary>
    public partial class Vxod : Page
    {
        public Vxod()
        {
            InitializeComponent();

        
        }


        private void OpenRegisterWindow_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Registracia());
        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text.Trim();
            string password = PasswordInput.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите логин и пароль!");
                return;
            }

            try
            {
                using (var db = new AvtoParkEntities())
                {
                    var user = db.Users.FirstOrDefault(u => u.Login == login);

                    if (user == null)
                    {
                        MessageBox.Show("Неверный логин или пароль!");
                        return;
                    }
                    string enteredHash = PasswordHelper.getHashPassword(password, user.Salt);

                    if (enteredHash != user.Password)
                    {
                        MessageBox.Show("Неверный логин или пароль!");
                        return;
                    }
                    var mainWindow = Application.Current.MainWindow as MainWindow;
                    if (mainWindow != null)
                    {
                        mainWindow.BtnVehicles.IsEnabled = true;
                        mainWindow.BtnDrivers.IsEnabled = true;
                        mainWindow.BtnRoutes.IsEnabled = true;

                        mainWindow.CurrentUser = user;
                    }

                    MessageBox.Show($"Добро пожаловать, {user.FIO}!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    LoginBox.Clear();
                    PasswordInput.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при авторизации:\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
