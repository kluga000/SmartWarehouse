using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SmartWarehouse
{
    public partial class MainWindow : Window
    {
        private ProductRepository _productRepo = new ProductRepository();
        public User CurrentUser { get; set; } = new Admin("admin_rari", "1234");

        public MainWindow()
        {
            InitializeComponent();

            Category tech = new Category("Техніка");
            Category laptops = new Category("Ноутбуки", tech);
            Category microwaves = new Category("Мікрохвильовки", tech);

            _productRepo.Add(new Product(1, "Lenovo Legion", "LNV-01", 45000, 5, laptops));
            _productRepo.Add(new Product(2, "Xiaomi MWB010-2A", "XAO-02", 3399, 25, microwaves));

            RefreshTable();
            this.Title = $"SmartWarehouse - Користувач: {CurrentUser.Login}";
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            var uniqueCategories = new Dictionary<string, Category>();
            foreach (var p in _productRepo.GetAll())
            {
                var current = p.Category;
                while (current != null)
                {
                    if (!uniqueCategories.ContainsKey(current.FullName))
                    {
                        uniqueCategories.Add(current.FullName, current);
                    }
                    current = current.ParentCategory;
                }
            }

            AddProductWindow addWin = new AddProductWindow(uniqueCategories.Values.ToList());
            addWin.Owner = this;

            if (addWin.ShowDialog() == true)
            {
                var newP = addWin.ResultProduct;

                MessageBoxResult confirmAdd = MessageBox.Show(
                    $"Ви впевнені, що хочете ДОДАТИ новий товар:\n\"{newP.Name}\" (Артикул: {newP.Article})?",
                    "Підтвердження операції",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question,
                    MessageBoxResult.No);

                if (confirmAdd == MessageBoxResult.Yes)
                {
                    var existing = _productRepo.Find(p => p.Article == newP.Article).FirstOrDefault();

                    if (existing != null)
                    {
                        existing.Quantity += newP.Quantity;
                        existing.IsDeleted = false;
                        MessageBox.Show("Кількість оновлена.");
                    }
                    else
                    {
                        int nextId = _productRepo.GetAll().Any() ? _productRepo.GetAll().Max(p => p.Id) + 1 : 1;
                        var productToAdd = new Product(nextId, newP.Name, newP.Article, newP.Price, newP.Quantity, newP.Category);
                        _productRepo.Add(productToAdd);
                        MessageBox.Show("Новий товар додано.");
                    }
                    RefreshTable();
                }
            }
        }

        private void btnIncrease_Click(object sender, RoutedEventArgs e)
        {
            if (dgProducts.SelectedItem is Product selected)
            {
                if (int.TryParse(txtAmountChange.Text, out int amount) && amount > 0)
                {
                    MessageBoxResult result = MessageBox.Show(
                        $"Ви впевнені, що хочете ДОДАТИ {amount} шт. до товару \"{selected.Name}\"?",
                        "Підтвердження зміни залишку",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question,
                        MessageBoxResult.No);

                    if (result == MessageBoxResult.Yes)
                    {
                        selected.AddQuantity(amount);
                        RefreshTable();
                    }
                }
                else
                {
                    MessageBox.Show("Введіть коректне число.");
                }
            }
            else
            {
                MessageBox.Show("Спочатку виберіть товар у таблиці.");
            }
        }

        private void btnDecrease_Click(object sender, RoutedEventArgs e)
        {
            if (dgProducts.SelectedItem is Product selected)
            {
                if (int.TryParse(txtAmountChange.Text, out int amount) && amount > 0)
                {
                    if (selected.Quantity >= amount)
                    {
                        MessageBoxResult result = MessageBox.Show(
                            $"Ви впевнені, що хочете СПИСАТИ {amount} шт. з товару \"{selected.Name}\"?",
                            "Підтвердження списання",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning,
                            MessageBoxResult.No);

                        if (result == MessageBoxResult.Yes)
                        {
                            selected.Quantity -= amount;
                            RefreshTable();
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Недостатньо товару! Доступно лише {selected.Quantity} шт.");
                    }
                }
                else
                {
                    MessageBox.Show("Введіть коректне число.");
                }
            }
            else
            {
                MessageBox.Show("Спочатку виберіть товар у таблиці.");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgProducts.SelectedItem is Product selected)
            {
                if (selected.IsDeleted)
                {
                    MessageBox.Show("Цей товар уже в архіві.");
                    return;
                }

                MessageBoxResult result = MessageBox.Show(
                    $"Ви впевнені, що хочете перенести в архів:\n\"{selected.Name}\"?",
                    "Підтвердження видалення",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning,
                    MessageBoxResult.No);

                if (result == MessageBoxResult.Yes)
                {
                    selected.IsDeleted = true;
                    RefreshTable();
                }
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string search = txtSearch.Text.ToLower();
            var results = _productRepo.Find(p =>
                p.Name.ToLower().Contains(search) || p.Article.ToLower().Contains(search)).ToList();

            dgProducts.ItemsSource = results.OrderBy(p => p.IsDeleted).ThenBy(p => p.Id).ToList();
        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            double total = _productRepo.GetAll().Where(p => !p.IsDeleted).Sum(p => p.Price * p.Quantity);
            MessageBox.Show($"Загальна вартість активних товарів: {total:N2} грн");
        }

        private void RefreshTable()
        {
            dgProducts.ItemsSource = null;
            dgProducts.ItemsSource = _productRepo.GetAll()
                .OrderBy(p => p.IsDeleted)
                .ThenBy(p => p.Id)
                .ToList();
        }
    }
}