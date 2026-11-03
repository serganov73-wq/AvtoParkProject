using Proekt3.Добавить_и_Редактировать;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

namespace Proekt3.ТранСредства__Водители_и_Рейсы
{
    /// <summary>
    /// Логика взаимодействия для RoutesPage.xaml
    /// </summary>
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

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (var db = new AvtoParkEntities())
                {
                    var routes = db.Routes.Include(r => r.Vehicles) .Include(r => r.Drivers).AsQueryable();

                    string search = SearchBox.Text.Trim();
                    if (!string.IsNullOrWhiteSpace(search))
                    {
                        routes = routes.Where(r => r.Drivers.FullName.Contains(search) ||r.Vehicles.LicensePlate.Contains(search));
                    }

                    RoutesGrid.ItemsSource = routes.ToList();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки: " + ex.Message);
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            AddRoutes add = new AddRoutes();
            if (add.ShowDialog() == true)
            {
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
            var result = MessageBox.Show($"Вы уверены, что хотите удалить рейс ?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);

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
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении:\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ApplyFilter(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void ResetFilter(object sender, RoutedEventArgs e)
        {
            SearchBox.Clear();
            SortBox.SelectedIndex = 0;
            LoadData();
        }
    }
}
