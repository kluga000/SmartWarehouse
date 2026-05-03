using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace SmartWarehouse
{
    public partial class MainWindow : Window
    {
        private ProductRepository _productRepo = new ProductRepository();
        public User CurrentUser { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += (s, e) =>
            {
                RefreshTable();
                this.Title = $"SmartWarehouse - Користувач: {CurrentUser?.Login ?? "Гість"}";

                // Обмеження прав доступу: приховуємо адмін-функції для звичайних користувачів
                if (CurrentUser?.Role == UserRole.User)
                {
                    btnDelete.Visibility = Visibility.Collapsed;
                    btnReport.Visibility = Visibility.Collapsed;
                    this.Title += " (Режим Менеджера)";
                }
            };
        }

        // Динамічна зміна вигляду кнопки видалення залежно від статусу вибраного товару
        private void dgProducts_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dgProducts.SelectedItem is Product selected)
            {
                if (selected.IsDeleted)
                {
                    btnDelete.Content = "Відновити/Видалити";
                    btnDelete.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#D1E7DD");
                }
                else
                {
                    btnDelete.Content = "Видалити";
                    btnDelete.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFE1E1");
                }
            }
            else
            {
                btnDelete.Content = "Відновити/Видалити";
                btnDelete.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFE1E1");
            }
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
                        uniqueCategories.Add(current.FullName, current);
                    current = current.ParentCategory;
                }
            }

            AddProductWindow addWin = new AddProductWindow(uniqueCategories.Values.ToList()) { Owner = this };

            if (addWin.ShowDialog() == true)
            {
                var newP = addWin.ResultProduct;
                var existing = _productRepo.Find(p => p.Article == newP.Article).FirstOrDefault();

                if (existing != null)
                {
                    if (existing.IsDeleted)
                    {
                        var result = MessageBox.Show($"Артикул {newP.Article} знайдено в архіві. Відновити його та додати кількість {newP.Quantity} шт.?",
                            "Товар в архіві", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            existing.IsDeleted = false;
                            existing.Quantity += newP.Quantity;
                            existing.Price = newP.Price; 
                        }
                        else return;
                    }
                    else
                    {
                        existing.Quantity += newP.Quantity;
                        existing.Price = newP.Price;
                        MessageBox.Show($"Кількість товару (арт. {newP.Article}) збільшена на {newP.Quantity} шт.");
                    }
                }
                else
                {
                    int nextId = _productRepo.GetAll().Any()
                        ? _productRepo.GetAll().Max(p => p.Id) + 1
                        : 1;

                    newP.Id = nextId;

                    _productRepo.Add(newP);
                }

                _productRepo.SaveChanges();
                RefreshTable();
            }
        }

        // Методи коригування кількості з перевіркою на статус архіву
        private void btnIncrease_Click(object sender, RoutedEventArgs e)
        {
            if (!(dgProducts.SelectedItem is Product selected)) return;
            if (selected.IsDeleted) { MessageBox.Show("Спочатку відновіть товар з архіву."); return; }

            if (int.TryParse(txtAmountChange.Text, out int amount) && amount > 0)
            {
                selected.AddQuantity(amount);
                _productRepo.SaveChanges();
                RefreshTable();
            }
        }

        private void btnDecrease_Click(object sender, RoutedEventArgs e)
        {
            if (!(dgProducts.SelectedItem is Product selected)) return;
            if (selected.IsDeleted) { MessageBox.Show("Архівний товар не можна списувати."); return; }

            if (int.TryParse(txtAmountChange.Text, out int amount) && amount > 0)
            {
                if (selected.Quantity >= amount)
                {
                    selected.Quantity -= amount;
                    _productRepo.SaveChanges();
                    RefreshTable();
                }
                else MessageBox.Show("Недостатньо товару на складі.");
            }
        }

        // Логіка видалення: в архів або остаточне видалення з репозиторію
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!(dgProducts.SelectedItem is Product selected))
            {
                MessageBox.Show("Виберіть товар у таблиці.");
                return;
            }

            if (selected.IsDeleted)
            {
                var result = MessageBox.Show($"Дія для \"{selected.Name}\":\nТАК — Відновити\nНІ — Видалити назавжди",
                    "Керування архівом", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes) { selected.IsDeleted = false; MessageBox.Show("Відновлено."); }
                else if (result == MessageBoxResult.No) { _productRepo.Remove(selected); MessageBox.Show("Видалено."); }
                else return;
            }
            else
            {
                if (MessageBox.Show("Перенести в архів?", "Архівування", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    selected.IsDeleted = true;
                    selected.Quantity = 0; 
                }
                else return;
            }

            _productRepo.SaveChanges();
            RefreshTable();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string search = txtSearch.Text.ToLower();
            var results = _productRepo.Find(p =>
                p.Name.ToLower().Contains(search) || p.Article.ToLower().Contains(search)).ToList();

            // Сортування: активні зверху, далі за ID
            dgProducts.ItemsSource = results.OrderBy(p => p.IsDeleted).ThenBy(p => p.Id).ToList();
        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            var active = _productRepo.GetAll().Where(p => !p.IsDeleted).ToList();

            // Генерація звіту лише на основі актуальних (не архівних) залишків
            string report = $"📊 ЗВІТ ПО СКЛАДУ\n" +
                            $"💰 Загальна вартість: {active.Sum(p => p.Price * p.Quantity):N2} грн\n" +
                            $"⚠️ Товарів «під нуль»: {active.Count(p => p.Quantity == 0)}\n\n" +
                            $"Статистика категорій:\n" +
                            string.Join("\n", active.GroupBy(p => p.Category.Name).Select(g => $"- {g.Key}: {g.Count()}"));

            MessageBox.Show(report, "Аналітика", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void RefreshTable()
        {
            dgProducts.ItemsSource = null;
            dgProducts.ItemsSource = _productRepo.GetAll()
                .OrderBy(p => p.IsDeleted) // Архівні записи завжди в кінці списку
                .ThenBy(p => p.Id)
                .ToList();
        }
    }
}