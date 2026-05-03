using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SmartWarehouse
{
    // Описує ієрархічну структуру категорій (дерево)
    public class Category
    {
        public string Name { get; set; }

        // Посилання на батьківський елемент для побудови ланцюжка FullName
        public Category ParentCategory { get; set; }

        // [JsonIgnore] критично важливий: запобігає циклічним посиланням при збереженні
        [JsonIgnore]
        public List<Category> SubCategories { get; set; } = new List<Category>();

        // Рекурсивна властивість для відображення повного шляху (напр. "Електроніка > Смартфони")
        public string FullName
        {
            get
            {
                return ParentCategory != null
                    ? $"{ParentCategory.FullName} > {Name}"
                    : Name;
            }
        }

        public Category() { }

        public Category(string name, Category parent = null)
        {
            Name = name;
            ParentCategory = parent;
            // Автоматична реєстрація в ієрархії при створенні підкатегорії
            if (parent != null) parent.SubCategories.Add(this);
        }

        // Перевизначення для коректного відображення у ComboBox
        public override string ToString() => Name;

        // Метод для вибірки товарів, що належать саме до цієї категорії
        public List<Product> GetProducts(List<Product> allProducts)
        {
            return allProducts.Where(p => p.Category == this).ToList();
        }
    }
}