using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace SmartWarehouse
{
    /// <summary>
    /// Модель товару з підтримкою сповіщень про зміну властивостей (для UI)
    /// </summary>
    public class Product : INotifyPropertyChanged
    {
        private int _quantity;
        private bool _isDeleted;

        public int Id { get; set; }

        [Required(ErrorMessage = "Назва товару обов'язкова")]
        public string Name { get; set; }

        public string Article { get; set; }

        // Використовується для фінансових розрахунків у звітах
        public double Price { get; set; }

        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged(); // Оновлює значення в DataGrid автоматично
            }
        }

        public Category Category { get; set; }

        // Поле для реалізації механізму «м'якого видалення» без втрати даних
        public bool IsDeleted
        {
            get => _isDeleted;
            set
            {
                _isDeleted = value;
                OnPropertyChanged();
            }
        }

        // Порожній конструктор необхідний для коректної роботи JSON-десеріалізатора
        public Product() { }

        public Product(int id, string name, string article, double price, int quantity, Category category)
        {
            Id = id;
            Name = name;
            Article = article;
            Price = price;
            Quantity = quantity;
            Category = category;
        }

        // Бізнес-логіка: безпечне поповнення залишків
        public void AddQuantity(int amount)
        {
            if (amount > 0) Quantity += amount;
        }

        // Реалізація інтерфейсу INotifyPropertyChanged для зв'язку Моделі з Інтерфейсом (WPF Binding)
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}