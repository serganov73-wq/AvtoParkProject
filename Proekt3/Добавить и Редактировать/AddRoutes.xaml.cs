using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.EMMA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Remoting.Lifetime;
using System.Text;
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
    /// Логика взаимодействия для AddRoutes.xaml
    /// </summary>
    public partial class AddRoutes : Window
    {
        public AddRoutes()
        {
            InitializeComponent();
            LoadComboBoxes();
        }

        private void LoadComboBoxes()
        {
            try
            {
                using (var db = new AvtoParkEntities())
                {
                    var availableVehicles = db.Vehicles.Where(v => v.Status == "Доступен").ToList();
                    CarBox.ItemsSource = availableVehicles;
                    var drivers = db.Drivers.ToList();
                    VoditelBox.ItemsSource = drivers;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки списков: " + ex.Message);
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (CarBox.SelectedValue == null || VoditelBox.SelectedValue == null)
            {
                MessageBox.Show("Пожалуйста, выберите машину и водителя!", "Ошибка");
                return;
            }

            int carId = (int)CarBox.SelectedValue;
            int driverId = (int)VoditelBox.SelectedValue;

            string start = StartLocationBox.Text.Trim();
            string end = EndLocationBox.Text.Trim();
            string distStr = DistanceBox.Text.Trim();

            if (StartDatePicker.SelectedDate == null || EndDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, выберите даты начала и окончания!", "Ошибка");
                return;
            }
            DateTime startDate = StartDatePicker.SelectedDate.Value;
            DateTime endDate = EndDatePicker.SelectedDate.Value;

            if (string.IsNullOrWhiteSpace(start) || string.IsNullOrWhiteSpace(end) || string.IsNullOrWhiteSpace(distStr))
            {
                MessageBox.Show("Пожалуйста, заполните все текстовые поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!int.TryParse(distStr, out int distance))
            {
                MessageBox.Show("Расстояние должно быть числом!", "Ошибка");
                return;
            }

            if (endDate < startDate)
            {
                MessageBox.Show("Дата окончания не может быть раньше даты начала!", "Ошибка");
                return;
            }

            try
            {
                using (var db = new AvtoParkEntities())
                {

                    var newRoutes = new Routes
                    {
                        VehicleId = carId,
                        DriverId = driverId,
                        StartDate = startDate,
                        EndDate = endDate,
                        StartLocation = start,
                        EndLocation = end,
                        Distance = distance
                    };

                    db.Routes.Add(newRoutes);

                    var vehicle = db.Vehicles.Find(carId);
                    if (vehicle != null)
                    {
                        vehicle.Status = "В рейсе";
                    }

                    db.SaveChanges();
                }

                MessageBox.Show("Рейс успешно добавлен! Машина отправлена в рейс!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении в БД:\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
