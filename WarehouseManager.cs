using System.Collections.Generic;
using System.Linq;

namespace SmartWarehouse
{
    public class WarehouseManager
    {
        public List<Product> Products { get; set; } = new List<Product>();
        public List<Category> Categories { get; set; } = new List<Category>();
        public void UpsertProduct(Product newProductData)
        {
            var existing = Products.FirstOrDefault(p => p.Article == newProductData.Article);
            if (existing != null)
            {
                existing.Quantity += newProductData.Quantity;
                existing.Price = newProductData.Price;
            }
            else
            {
                Products.Add(newProductData);
            }
        }
    }
}