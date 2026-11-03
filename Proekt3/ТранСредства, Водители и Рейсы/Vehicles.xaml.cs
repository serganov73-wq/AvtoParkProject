using Proekt3.Добавить_и_Редактировать;
using System;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Windows;
using System.Windows.Controls;
// ... остальные using ...

namespace Proekt3.ТранСредства__Водители_и_Рейсы
{
    public partial class Vehicles : Page
    {
        private Users _currentUser;
        public Vehicles(Users user)
        {
            InitializeComponent();
            _currentUser = user;

            if (_currentUser.Role != "Admin")
            {
                AddBtn.Visibility = Visibility.Collapsed;
                EditBtn.Visibility = Visibility.Collapsed;
                DeleteBtn.Visibility = Visibility.Collapsed;
            }


            LoadModels();
            LoadData();
        }

        private void LoadModels()
        {
            try
            {
                using (var db = new AvtoParkEntities())
                {
                    var models = db.Vehicles.Select(v => v.Model).Distinct().OrderBy(m => m).ToList();
                    models.Insert(0, "Все модели");
                    ModelBox.ItemsSource = models;
                    ModelBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadData()
        {
            try
            {
                using (var db = new AvtoParkEntities())
                {
                    var query = db.Vehicles.AsQueryable();

                    string search = SearchBox.Text.Trim();
                    if (!string.IsNullOrWhiteSpace(search))
                    {
                        query = query.Where(v => v.LicensePlate.Contains(search));
                    }

                    if (ModelBox.SelectedIndex > 0)
                    {
                        string selectedModel = ModelBox.SelectedItem.ToString();
                        query = query.Where(v => v.Model == selectedModel);
                    }

                    var availItem = AvailabilityBox.SelectedItem as ComboBoxItem;
                    if (availItem != null && availItem.Content.ToString() != "Все")
                    {
                        string status = availItem.Content.ToString();
                        query = query.Where(v => v.Status == status);
                    }

                    if (SortBox.SelectedIndex == 1)
                    {
                        query = query.OrderByDescending(v => v.Model);
                    }
                    else
                    {
                        query = query.OrderBy(v => v.Model);
                    }

                    VehiclesGrid.ItemsSource = query.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void ApplyFilter(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void ResetFilter(object sender, RoutedEventArgs e)
        {
            SearchBox.Clear();
            ModelBox.SelectedIndex = 0;
            AvailabilityBox.SelectedIndex = 0;
            SortBox.SelectedIndex = 0;
            LoadData();
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            AddVehicles add = new AddVehicles();
            if (add.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            if (VehiclesGrid.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите машину для редактирования!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedVehicle = VehiclesGrid.SelectedItem as Proekt3.Vehicles;
            RedaktVehicles red = new RedaktVehicles(selectedVehicle);
            if (red.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (VehiclesGrid.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите машину для удаления!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedVehicle = VehiclesGrid.SelectedItem as Proekt3.Vehicles;
            var result = MessageBox.Show("Вы уверены, что хотите удалить?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new AvtoParkEntities())
                    {
                        if (db.Routes.Any(r => r.VehicleId == selectedVehicle.Id))
                        {
                            MessageBox.Show("Невозможно удалить машину, так как у неё есть назначенные рейсы!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        var vehicleToDelete = db.Vehicles.Find(selectedVehicle.Id);

                        if (vehicleToDelete != null)
                        {
                            db.Vehicles.Remove(vehicleToDelete);
                            db.SaveChanges();
                        }
                    }

                    MessageBox.Show("Машина успешно удалена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении:\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }

        private void ReportBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Сохранение отчета",
                Filter = "Excel Worksheets (*.xlsx)|*.xlsx",
                DefaultExt = ".xlsx",
                FileName = $"Otchet_Probeg_{DateTime.Now:dd-MM-yyyy_HH-mm}"
            };

            if (dialog.ShowDialog() == false) return;

            try
            {
                using (var db = new AvtoParkEntities())
                {
                    var vehicles = db.Vehicles.ToList();

                    using (var workbook = new ClosedXML.Excel.XLWorkbook())
                    {
                        var sheet = workbook.AddWorksheet("Пробег");

                        sheet.Cell(1, 1).Value = "Гос. номер";
                        sheet.Cell(1, 2).Value = "Модель";
                        sheet.Cell(1, 3).Value = "Пробег (км)";

                        int row = 2;
                        foreach (var v in vehicles)
                        {
                            sheet.Cell(row, 1).Value = v.LicensePlate;
                            sheet.Cell(row, 2).Value = v.Model;
                            sheet.Cell(row, 3).Value = v.Mileage;
                            row++;
                        }

                        int totalMileage = vehicles.Sum(v => v.Mileage ?? 0); 

                        sheet.Cell(row + 1, 2).Value = "Общий пробег:";
                        sheet.Cell(row + 1, 2).Style.Font.Bold = true;
                        sheet.Cell(row + 1, 3).Value = totalMileage;
                        sheet.Cell(row + 1, 3).Style.Font.Bold = true;

                        sheet.Columns().AdjustToContents();
                        workbook.SaveAs(dialog.FileName);
                        MessageBox.Show("Отчет успешно сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при создании отчета:\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}