using Proekt3.Добавить_и_Редактировать;
using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Proekt3.ТранСредства__Водители_и_Рейсы
{
    public partial class RoutesPage : Page
    {
        private Users _currentUser;

        public RoutesPage(Users user)
        {
            InitializeComponent();
            _currentUser = user;

            if (_currentUser.Role != "Admin")
            {
                AddBtn.Visibility = Visibility.Collapsed;
                DeleteBtn.Visibility = Visibility.Collapsed;
            }

            LoadFilterComboBoxes();
            LoadData();
        }

        private void LoadFilterComboBoxes()
        {
            try
            {
                using (var db = new AvtoParkEntities())
                {
                    var vehicles = db.Vehicles.ToList();
                    vehicles.Insert(0, new Proekt3.Vehicles { Id = 0, LicensePlate = "Все машины" });
                    VehicleFilterBox.ItemsSource = vehicles;
                    VehicleFilterBox.SelectedIndex = 0;

                    var drivers = db.Drivers.ToList();
                    drivers.Insert(0, new Proekt3.Drivers { Id = 0, FullName = "Все водители" });
                    DriverFilterBox.ItemsSource = drivers;
                    DriverFilterBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки фильтров: " + ex.Message);
            }
        }

        private void LoadData()
        {
            try
            {
                using (var db = new AvtoParkEntities())
                {
                    var query = db.Routes.Include(r => r.Vehicles).Include(r => r.Drivers).AsQueryable();

                    if (VehicleFilterBox.SelectedValue != null && (int)VehicleFilterBox.SelectedValue > 0)
                    {
                        int vId = (int)VehicleFilterBox.SelectedValue;
                        query = query.Where(r => r.VehicleId == vId);
                    }

                    if (DriverFilterBox.SelectedValue != null && (int)DriverFilterBox.SelectedValue > 0)
                    {
                        int dId = (int)DriverFilterBox.SelectedValue;
                        query = query.Where(r => r.DriverId == dId);
                    }

                    if (StartDateFilter.SelectedDate != null)
                    {
                        DateTime start = StartDateFilter.SelectedDate.Value;
                        query = query.Where(r => r.StartDate >= start);
                    }

                    if (EndDateFilter.SelectedDate != null)
                    {
                        DateTime end = EndDateFilter.SelectedDate.Value;
                        query = query.Where(r => r.EndDate <= end);
                    }

                    RoutesGrid.ItemsSource = query.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки: " + ex.Message);
            }
        }

        private void ApplyFilter(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void ResetFilter(object sender, RoutedEventArgs e)
        {
            VehicleFilterBox.SelectedIndex = 0;
            DriverFilterBox.SelectedIndex = 0;
            StartDateFilter.SelectedDate = null;
            EndDateFilter.SelectedDate = null;
            LoadData();
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            AddRoutes add = new AddRoutes();
            if (add.ShowDialog() == true)
            {
                LoadFilterComboBoxes();
                LoadData();
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (RoutesGrid.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите рейс для удаления!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedRoutes = RoutesGrid.SelectedItem as Proekt3.Routes;
            var result = MessageBox.Show($"Вы уверены, что хотите удалить рейс?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new AvtoParkEntities())
                    {
                        var rouresToDelete = db.Routes.Find(selectedRoutes.Id);

                        if (rouresToDelete != null)
                        {
                            var vehicle = db.Vehicles.Find(rouresToDelete.VehicleId);
                            if (vehicle != null)
                            {
                                vehicle.Status = "Доступен";
                            }
                            db.Routes.Remove(rouresToDelete);
                            db.SaveChanges();
                        }
                    }
                    MessageBox.Show("Рейс успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadFilterComboBoxes();
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении:\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}