using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SmartWarehouse
{
    public partial class AddProductWindow : Window
    {
        public Product ResultProduct { get; private set; }
        private List<Category> _existingCategories;

        public AddProductWindow(List<Category> categories)
        {
            InitializeComponent();
            _existingCategories = categories;
            cbCategories.ItemsSource = _existingCategories;
            cbCategories.IsEditable = true;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string catName = cbCategories.Text; 
                if (string.IsNullOrWhiteSpace(catName) || string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Заповніть назву та категорію!");
                    return;
                }

                Category selectedCat = _existingCategories.FirstOrDefault(c => c.Name == catName);
                if (selectedCat == null)
                {
                    selectedCat = new Category(catName);
                }

                ResultProduct = new Product(0, txtName.Text, txtArticle.Text,
                    double.Parse(txtPrice.Text), int.Parse(txtQuantity.Text), selectedCat);

                this.DialogResult = true;
            }
            catch { MessageBox.Show("Помилка вводу цифр!"); }
        }
    }
}