using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Proekt3.Добавить_и_Редактировать
{
    public partial class AddDrivers : Window
    {
        public AddDrivers()
        {
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            string fio = FIOBox.Text.Trim(); 
            string numberby = NumberBYBox.Text.Trim();
            string email = EmailBox.Text.Trim();
            string phone = PhoneBox.Text.Trim();
            string experienceStr = ExperienceBox.Text.Trim();
            string parol = ParolBox.Password;

            if (string.IsNullOrWhiteSpace(fio) ||
                string.IsNullOrWhiteSpace(numberby) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(experienceStr) ||
                string.IsNullOrWhiteSpace(parol))
            {
                MessageBox.Show("Пожалуйста, заполните все поля, включая пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(experienceStr, out int experience))
            {
                MessageBox.Show("Стаж должен быть числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new AvtoParkEntities())
                {
                    if (db.Drivers.Any(d => d.Email == email))
                    {
                        MessageBox.Show("Водитель с таким Email уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    string salt = PasswordHelper.getSaltPassword();
                    string hashPassword = PasswordHelper.getHashPassword(parol, salt);

                    var newDriver = new Drivers
                    {
                        FullName = fio,
                        LicenseNumber = numberby,
                        PhoneNumber = phone,
                        Experience = experience, 
                        Email = email,
                        Salt = salt,
                        Password = hashPassword, 
                        Role = "User" 
                    };

                    db.Drivers.Add(newDriver);
                    db.SaveChanges();
                }

                MessageBox.Show("Водитель успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                string inner = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MessageBox.Show("Ошибка при сохранении в БД:\n" + inner, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}