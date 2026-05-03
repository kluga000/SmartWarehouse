using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.ComponentModel.DataAnnotations;

namespace SmartWarehouse
{
    public partial class AddProductWindow : Window
    {
        // Властивість для передачі створеного товару назад у головне вікно
        public Product ResultProduct { get; private set; }
        private List<Category> _existingCategories;

        public AddProductWindow(List<Category> categories)
        {
            InitializeComponent();
            _existingCategories = categories;

            // Налаштування списку категорій: дозволяємо як вибір існуючих, так і введення нових
            cbCategories.ItemsSource = _existingCategories;
            cbCategories.IsEditable = true;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Первинна перевірка на порожні поля перед спробою створення об'єкта
                if (string.IsNullOrWhiteSpace(txtName.Text) ||
                    string.IsNullOrWhiteSpace(txtArticle.Text) ||
                    string.IsNullOrWhiteSpace(cbCategories.Text))
                {
                    MessageBox.Show("Будь ласка, заповніть усі обов'язкові поля!", "Увага");
                    return;
                }

                // Валідація числових значень: ціна та кількість мають відповідати формату
                if (!double.TryParse(txtPrice.Text, out double price) || price <= 0)
                {
                    MessageBox.Show("Ціна повинна бути числом більше нуля.", "Помилка вводу");
                    return;
                }

                if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity < 0)
                {
                    MessageBox.Show("Кількість не може бути від’ємною.", "Помилка вводу");
                    return;
                }

                // Робота з категорією: шукаємо збіг у списку або створюємо нову
                string catName = cbCategories.Text;
                Category selectedCat = _existingCategories.FirstOrDefault(c => c.Name == catName)
                                       ?? new Category(catName);

                // Створення тимчасового об'єкта для повної валідації через атрибути моделі
                var tempProduct = new Product(0, txtName.Text, txtArticle.Text, price, quantity, selectedCat);

                // Використання системної валідації DataAnnotations (якщо в класі Product є обмеження [Required], [Range] тощо)
                var context = new ValidationContext(tempProduct);
                var results = new List<ValidationResult>();

                if (!Validator.TryValidateObject(tempProduct, context, results, true))
                {
                    string allErrors = string.Join("\n", results.Select(r => r.ErrorMessage));
                    MessageBox.Show(allErrors, "Помилка моделі даних");
                    return;
                }

                // Якщо всі перевірки пройдені, зберігаємо результат і закриваємо вікно
                ResultProduct = tempProduct;
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                // Запобіжний захід на випадок непередбачуваних збоїв при роботі з типами даних
                MessageBox.Show($"Не вдалося зберегти товар: {ex.Message}", "Помилка");
            }
        }
    }
}