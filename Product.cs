using System;

namespace SmartWarehouse
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Article { get; set; }
        public double Price { get; set; }
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