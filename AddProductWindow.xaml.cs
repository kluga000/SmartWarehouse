using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.ComponentModel.DataAnnotations;

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
                if (string.IsNullOrWhiteSpace(txtName.Text) ||
                    string.IsNullOrWhiteSpace(txtArticle.Text) ||
                    string.IsNullOrWhiteSpace(cbCategories.Text))
                {
                    MessageBox.Show("Будь ласка, заповніть усі поля!", "Помилка");
                    return;
                }

                if (!double.TryParse(txtPrice.Text, out double price) || price <= 0)
                {
                    MessageBox.Show("Ціна повинна бути додатним числом!", "Помилка вводу");
                    return;
                }

                if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity < 0)
                {
                    MessageBox.Show("Кількість не може бути від'ємною!", "Помилка вводу");
                    return;
                }

                string catName = cbCategories.Text;
                Category selectedCat = _existingCategories.FirstOrDefault(c => c.Name == catName);
                if (selectedCat == null)
                {
                    selectedCat = new Category(catName);
                }

                var tempProduct = new Product(0, txtName.Text, txtArticle.Text, price, quantity, selectedCat);

                var context = new ValidationContext(tempProduct);
                var results = new List<ValidationResult>();

                if (!Validator.TryValidateObject(tempProduct, context, results, true))
                {
                    string allErrors = string.Join("\n", results.Select(r => r.ErrorMessage));
                    MessageBox.Show(allErrors, "Помилка валідації");
                    return;
                }

                ResultProduct = tempProduct;
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Виникла помилка: {ex.Message}", "Критична помилка");
            }
        }
    }
}