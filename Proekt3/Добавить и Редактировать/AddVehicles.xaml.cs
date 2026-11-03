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
using System.Windows.Shapes;

namespace Proekt3.Добавить_и_Редактировать
{
    /// <summary>
    /// Логика взаимодействия для AddVehicles.xaml
    /// </summary>
    public partial class AddVehicles : Window
    {
        public AddVehicles()
        {
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            string licensePlate = LicensePlateBox.Text.Trim().ToUpper(); 
            string model = ModelBox.Text.Trim();
            string manufacturer = ManufacturerBox.Text.Trim();
            string yearText = YearBox.Text.Trim();
            string mileageText = MileageBox.Text.Trim();
            string status = (StatusBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrWhiteSpace(licensePlate) ||
                string.IsNullOrWhiteSpace(model) ||
                string.IsNullOrWhiteSpace(manufacturer) ||
                string.IsNullOrWhiteSpace(yearText) ||
                string.IsNullOrWhiteSpace(mileageText))
            {
                MessageBox.Show("Пожалуйста, заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!Regex.IsMatch(licensePlate, @"^[А-ЯA-Z]{1}[0-9]{3}[А-ЯA-Z]{2}[0-9]{2,3}$"))
            {
                MessageBox.Show("Неверный формат гос. номера! Пример: А123ВС77", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(yearText, out int year) || year < 1900 || year > DateTime.Now.Year + 1)
            {
                MessageBox.Show("Введите корректный год выпуска (например, 2020)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(mileageText, out int mileage) || mileage < 0)
            {
                MessageBox.Show("Введите корректный пробег (целое число больше 0)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new AvtoParkEntities())
                {
                    if (db.Vehicles.Any(v => v.LicensePlate == licensePlate))
                    {
                        MessageBox.Show("Машина с таким гос. номером уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var newVehicle = new Vehicles
                    {
                        LicensePlate = licensePlate,
                        Model = model,
                        Manufacturer = manufacturer,
                        YearOfManufacture = year,
                        Mileage = mileage,
                        Status = status
                    };

                    db.Vehicles.Add(newVehicle);
                    db.SaveChanges();
                }

                MessageBox.Show("Машина успешно добавлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении в БД:\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
     }
}
