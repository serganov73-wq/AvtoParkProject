using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Proekt3.Добавить_и_Редактировать
{
    public partial class RedaktDrivers : Window
    {
        private Drivers _currentDriver; 

        public RedaktDrivers(Drivers driver)
        {
            InitializeComponent();
            _currentDriver = driver;

            FIOBox.Text = driver.FullName;
            NumberBYBox.Text = driver.LicenseNumber;
            PhoneBox.Text = driver.PhoneNumber;
            ExperienceBox.Text = driver.Experience.ToString();
            EmailBox.Text = driver.Email;
        }

        private void Redakr_Click(object sender, RoutedEventArgs e)
        {
            string fio = FIOBox.Text.Trim();
            string numberby = NumberBYBox.Text.Trim();
            string email = EmailBox.Text.Trim();
            string phone = PhoneBox.Text.Trim();
            string experienceStr = ExperienceBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(fio) || string.IsNullOrWhiteSpace(numberby) ||
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(experienceStr))
            {
                MessageBox.Show("Пожалуйста, заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    if (db.Drivers.Any(d => d.Email == email && d.Id != _currentDriver.Id))
                    {
                        MessageBox.Show("Другой водитель с таким Email уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var driverToUpdate = db.Drivers.Find(_currentDriver.Id);

                    if (driverToUpdate != null)
                    {
                        driverToUpdate.FullName = fio;
                        driverToUpdate.LicenseNumber = numberby;
                        driverToUpdate.PhoneNumber = phone;
                        driverToUpdate.Experience = experience;
                        driverToUpdate.Email = email;

                        db.SaveChanges();
                    }
                }

                MessageBox.Show("Данные водителя успешно изменены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true; 
            }
            catch (Exception ex)
            {
                string inner = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MessageBox.Show("Ошибка при сохранении:\n" + inner, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}