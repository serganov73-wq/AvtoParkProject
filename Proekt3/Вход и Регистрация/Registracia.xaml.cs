using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для Registracia.xaml
    /// </summary>
    public partial class Registracia : Page
    {
        public string RegisterLogin { get; private set; }
        public Registracia()
        {
            InitializeComponent();
        }

        private void RegisterClick(object sender, RoutedEventArgs e)
        {
            FIOError.Text = "";
            LoginError.Text = "";
            MailError.Text = "";
            PasswordError.Text = "";
            PasswordRepeatError.Text = "";

            bool hasErrors = false;

            string fio = FIOBox.Text.Trim();
            string login = LoginBox.Text.Trim();
            string email = MailBox.Text.Trim();
            string password = PasswordInput.Password;
            string repeatPassword = PasswordRepeatInput.Password;

            if (fio.Length < 5 || fio.Length > 100)
            {
                FIOError.Text = "ФИО должно содержать от 5 до 100 символов";
                hasErrors = true;
            }

            if (!Regex.IsMatch(login, @"^[a-zA-Z0-9_]{4,20}$"))
            {
                LoginError.Text = "От 4 до 20 символов. Только латинские буквы, цифры и _";
                hasErrors = true;
            }

            if (string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MailError.Text = "Неверный формат электронной почты";
                hasErrors = true;
            }

            if (password.Length < 8 || password.Length > 128 ||
                !password.Any(char.IsUpper) || !password.Any(char.IsLower) || !password.Any(char.IsDigit))
            {
                PasswordError.Text = "От 8 до 128 символов. Нужна хотя бы 1 заглавная, 1 строчная и 1 цифра";
                hasErrors = true;
            }

            if (password != repeatPassword)
            {
                PasswordRepeatError.Text = "Пароли не совпадают";
                hasErrors = true;
            }

            if (hasErrors) return;

            try
            {
                using (var db = new AvtoParkEntities())
                {
                    if (db.Users.Any(u => u.Login == login))
                    {
                        LoginError.Text = "Такой логин уже существует";
                        hasErrors = true;
                    }

                    if (db.Users.Any(u => u.Email == email))
                    {
                        MailError.Text = "Эта почта уже зарегистрирована";
                        hasErrors = true;
                    }

                    if (hasErrors) return;

                    var selectedItem = RoleBox.SelectedItem as ComboBoxItem;
                    string rolepoYm = "User";

                    if(selectedItem != null)
                    {
                        string selectedRole = selectedItem.Content.ToString();

                        if(selectedRole == "Администратор")
                        {
                            rolepoYm = "Admin";
                        }
                    }

                    string salt = PasswordHelper.getSaltPassword();
                    var user = new Users
                    {
                        FIO = fio,
                        Login = login,
                        Email = email,
                        Salt = salt,
                        Password = PasswordHelper.getHashPassword(password, salt),
                        Role = rolepoYm
                    };

                    db.Users.Add(user);
                    db.SaveChanges();
                }

                RegisterLogin = login;
                MessageBox.Show("Регистрация пройдена успешно! Выполните авторизацию.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Exception realError = ex;
                while (realError.InnerException != null)
                {
                    realError = realError.InnerException;
                }

                MessageBox.Show($"НАСТОЯЩАЯ ОШИБКА SQL:\n{realError.Message}", "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Vxod());
        }
    }
}
