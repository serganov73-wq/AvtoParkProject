using Proekt3.Добавить_и_Редактировать;
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

namespace Proekt3.ТранСредства__Водители_и_Рейсы
{
    /// <summary>
    /// Логика взаимодействия для DriversPage.xaml
    /// </summary>
    public partial class DriversPage : Page
    {
        private Users _currentUser;
        public DriversPage(Users user)
        {
            InitializeComponent();
            _currentUser = user;

            if (_currentUser.Role != "Admin")
            {
                AddBtn.Visibility = Visibility.Collapsed;
                EditBtn.Visibility = Visibility.Collapsed;
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
                    var query = db.Drivers.AsQueryable();

                    string search = SearchBox.Text.Trim();
                    if (!string.IsNullOrWhiteSpace(search))
                    {
                        query = query.Where(d => d.FullName.Contains(search));
                    }

                    if (SortBox.SelectedIndex == 1) 
                    {
                        query = query.OrderByDescending(d => d.FullName);
                    }
                    else 
                    {
                        query = query.OrderBy(d => d.FullName);
                    }

                    DriversGrid.ItemsSource = query.ToList();
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
            SearchBox.Clear();
            SortBox.SelectedIndex = 0;
            LoadData();
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            AddDrivers add = new AddDrivers();
            if (add.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DriversGrid.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите водителя для редактирования!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedDrivers = DriversGrid.SelectedItem as Proekt3.Drivers;
            RedaktDrivers red = new RedaktDrivers(selectedDrivers);
            if (red.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DriversGrid.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите водителя для удаления!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedDrivers = DriversGrid.SelectedItem as Proekt3.Drivers;
            var result = MessageBox.Show($"Вы уверены, что хотите удалить водителя {selectedDrivers.FullName}?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new AvtoParkEntities())
                    {
                        if (db.Routes.Any(r => r.DriverId == selectedDrivers.Id))
                        {
                            MessageBox.Show("Невозможно удалить водителя, так как у него есть рейсы!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        var driversToDelete = db.Drivers.Find(selectedDrivers.Id);

                        if (driversToDelete != null)
                        {
                            db.Drivers.Remove(driversToDelete);
                            db.SaveChanges();
                        }
                    }
                    MessageBox.Show("Водитель успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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
