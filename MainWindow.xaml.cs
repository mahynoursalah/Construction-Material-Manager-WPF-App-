using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ConstructionMaterialManager.models; // ✅
using Microsoft.Win32;
using System.IO;
using System.Text.Json;


namespace ConstructionMaterialManager
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Material> _materials;
        private ObservableCollection<Order> _orders;

        public MainWindow()
        {
            InitializeComponent();

            _materials = new ObservableCollection<Material>();
            _orders = new ObservableCollection<Order>();

          
            MaterialsDataGrid.ItemsSource = _materials;

            LoadDefaultMaterials();

            UpdateSummaryCards();
            UpdateStatusBar();
            StatusTextBlock.Text = "Ready";
        }

        private void AddMaterial_Click(object sender, RoutedEventArgs e)
        {
            // 1) Read inputs
            string name = NameTextBox.Text?.Trim();
            string category = (CategoryComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            string unit = (UnitComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();

            // 2) Validate name
            if (string.IsNullOrWhiteSpace(name))
            {
                MarkInvalid(NameTextBox, "Name is required");
                return;
            }

            // Not duplicate
            if (_materials.Any(m => string.Equals(m.Name, name, StringComparison.OrdinalIgnoreCase)))
            {
                MarkInvalid(NameTextBox, "Name already exists");
                return;
            }


            MarkValid(NameTextBox);

            // 3) Validate category/unit
            if (string.IsNullOrWhiteSpace(category))
            {
                MessageBox.Show("Please select a category.");
                return;
            }

            if (string.IsNullOrWhiteSpace(unit))
            {
                MessageBox.Show("Please select a unit.");
                return;
            }

            // 4) Validate price
            if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price <= 0)
            {
                MarkInvalid(PriceTextBox, "Unit price must be a positive number");
                return;
            }

            MarkValid(PriceTextBox);

            // 5) Add material
            _materials.Add(new Material
            {
                Name = name,
                Category = category,
                Unit = unit,
                UnitPrice = price
            });

            // 6) Clear inputs
            NameTextBox.Clear();
            PriceTextBox.Clear();
            CategoryComboBox.SelectedIndex = -1;
            UnitComboBox.SelectedIndex = -1;

            // 7) Update UI summaries
            UpdateSummaryCards();
            UpdateStatusBar();

            StatusTextBlock.Text = "Material added.";
        }
        private void MarkInvalid(Control control, string message)
        {
            control.BorderBrush = System.Windows.Media.Brushes.Red;
            control.ToolTip = message;
        }

        private void MarkValid(Control control)
        {
            control.BorderBrush = System.Windows.Media.Brushes.Gray;
            control.ToolTip = null;
        }
        private void DeleteMaterial_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var material = button.Tag as Material;

            if (material == null) return;

            var result = MessageBox.Show(
                $"Delete '{material.Name}' ?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            _materials.Remove(material);

            UpdateSummaryCards();
            UpdateStatusBar();

            StatusTextBlock.Text = "Material deleted.";
        }

        private void OpenCalculator_Click(object sender, RoutedEventArgs e)
        {
            var win = new CalculatorWindow(_materials, _orders);
            win.ShowDialog();
            UpdateSummaryCards();
            UpdateStatusBar();
        }

        private void OpenOrders_Click(object sender, RoutedEventArgs e)
        {
            OrdersWindow window = new OrdersWindow(_orders);
            window.ShowDialog();

            UpdateSummaryCards();
            UpdateStatusBar();
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON files (*.json)|*.json";
            saveFileDialog.FileName = "data.json";

            if (saveFileDialog.ShowDialog() == true)
            {
                Appdata data = new Appdata
                {
                    Materials = _materials.ToList(),
                    Orders = _orders.ToList()
                };

                string json = JsonSerializer.Serialize(data, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(saveFileDialog.FileName, json);

                StatusTextBlock.Text = $"Last Saved: {DateTime.Now:dd/MM/yyyy HH:mm}";
                MessageBox.Show("Saved successfully.");
            }
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON files (*.json)|*.json";

            if (openFileDialog.ShowDialog() == true)
            {
                string json = File.ReadAllText(openFileDialog.FileName);

                Appdata data = JsonSerializer.Deserialize<Appdata>(json);

                if (data != null)
                {
                    _materials.Clear();
                    _orders.Clear();

                    if (data.Materials != null)
                    {
                        foreach (Material material in data.Materials)
                        {
                            _materials.Add(material);
                        }
                    }

                    if (data.Orders != null)
                    {
                        foreach (Order order in data.Orders)
                        {
                            _orders.Add(order);
                        }
                    }

                    MaterialsDataGrid.ItemsSource = _materials;

                    UpdateSummaryCards();
                    UpdateStatusBar();

                    StatusTextBlock.Text = $"Loaded: {DateTime.Now:dd/MM/yyyy HH:mm}";
                    MessageBox.Show("Loaded successfully.");
                }
            }
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Do you want to save before closing?",
                "Exit",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Cancel)
                return;

            if (result == MessageBoxResult.Yes)
            {
                Save_Click(sender, e);
            }

            Close();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Construction Material Manager\nWPF Final Project", "About");
        }

        private void UpdateSummaryCards()
        {
            CatalogCountTextBlock.Text = $"{_materials.Count} Materials";
            OrdersCountTextBlock.Text = $"{_orders.Count} Orders";
            TotalSpentTextBlock.Text = $"EGP {_orders.Sum(o => o.Total):N2}";
        }

        private void UpdateStatusBar()
        {
            MaterialsCountTextBlock.Text = $"Materials: {_materials.Count}";
        }
        private void LoadDefaultMaterials()
        {
            _materials.Add(new Material { Name = "Cement", Category = "Concrete", Unit = "Ton", UnitPrice = 2500 });
            _materials.Add(new Material { Name = "Sand", Category = "Concrete", Unit = "m³", UnitPrice = 350 });
            _materials.Add(new Material { Name = "Gravel", Category = "Concrete", Unit = "m³", UnitPrice = 450 });

            _materials.Add(new Material { Name = "Rebar 12mm", Category = "Steel", Unit = "Ton", UnitPrice = 38000 });
            _materials.Add(new Material { Name = "Rebar 16mm", Category = "Steel", Unit = "Ton", UnitPrice = 37500 });

            _materials.Add(new Material { Name = "Emulsion Paint", Category = "Paint", Unit = "Liter", UnitPrice = 180 });
            _materials.Add(new Material { Name = "Oil Paint", Category = "Paint", Unit = "Liter", UnitPrice = 280 });

            _materials.Add(new Material { Name = "Ceramic 60x60", Category = "Tiles", Unit = "m²", UnitPrice = 320 });
            _materials.Add(new Material { Name = "Porcelain 80x80", Category = "Tiles", Unit = "m²", UnitPrice = 550 });

            _materials.Add(new Material { Name = "Nails 5cm", Category = "General", Unit = "kg", UnitPrice = 85 });
        }
    }
}