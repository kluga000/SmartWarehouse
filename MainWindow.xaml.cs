using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace SmartWarehouse
{
    public partial class MainWindow : Window
    {
        public WarehouseManager manager = new WarehouseManager();

        public User CurrentUser { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Category tech = new Category("Електроніка");
            Category laptops = new Category("Ноутбуки", tech);
            Category microwaves = new Category("Мікрохвильовки", tech);

            CurrentUser = new Admin("admin_rari", "1234");

            Product testProduct = new Product(1, "Lenovo Legion", "LNV-01", 45000, 5, laptops);
            manager.UpsertProduct(testProduct);
            
            manager.UpsertProduct(new Product(2, "Xiaomi MWB010-2A", "XAO-02", 3399, 25, microwaves));

            manager.UpsertProduct(new Product(1, "Lenovo Legion", "LNV-01", 45000, 5, laptops));

            dgProducts.ItemsSource = null;
            dgProducts.ItemsSource = manager.Products; 

            dgProducts.ItemsSource = manager.Products;

            this.Title = $"SmartWarehouse - Користувач: {CurrentUser.Login}";
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchText = txtSearch.Text.ToLower();

            var filtered = manager.Products.Where(p =>
                p.Name.ToLower().Contains(searchText) ||
                p.Article.ToLower().Contains(searchText) ||
                (p.Category != null && p.Category.Name.ToLower().Contains(searchText))
            ).ToList();

            dgProducts.ItemsSource = filtered;
        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            double total = manager.Products.Sum(p => p.Price * p.Quantity);
            MessageBox.Show($"Загальна вартість товарів на складі: {total:N2} грн.", "Звіт");
        }
    }
}