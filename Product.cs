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
            string categoryName = Category != null ? Category.Name : "Без категорії";
            return $"{Name} [{categoryName}] (Артикул: {Article}) - Ціна: {Price}, Кількість: {Quantity}";
        }

        public void AddQuantity(int amount)
        {
            if (amount > 0)
                Quantity += amount;
        }
    }
}
