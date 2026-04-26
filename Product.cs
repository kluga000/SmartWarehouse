using System;
using System.ComponentModel.DataAnnotations;

namespace SmartWarehouse
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва товару обов'язкова")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Назва має бути від 2 до 100 символів")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Артикул обов'язковий")]
        [RegularExpression(@"^[A-Za-z0-9-]+$", ErrorMessage = "Артикул може містити лише букви, цифри та дефіс")]
        public string Article { get; set; }

        [Range(0.01, 1000000, ErrorMessage = "Ціна має бути більшою за нуль")]
        public double Price { get; set; }

        [Range(0, 100000, ErrorMessage = "Кількість не може бути від'ємною")]
        public int Quantity { get; set; }

        public Category Category { get; set; }

        public bool IsDeleted { get; set; } = false;

        public Product(int id, string name, string article, double price, int quantity, Category category)
        {
            Id = id;
            Name = name;
            Article = article;
            Price = price;
            Quantity = quantity;
            Category = category;
        }

        public string GetInfo()
        {
            string categoryName = Category != null ? Category.FullName : "Без категорії";
            string status = IsDeleted ? " [АРХІВ]" : "";
            return $"{Name}{status} [{categoryName}] (Артикул: {Article}) - Ціна: {Price}, Кількість: {Quantity}";
        }

        public void AddQuantity(int amount)
        {
            if (amount > 0)
                Quantity += amount;
        }
    }
}