using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SmartWarehouse
{
    public class Category
    {
        public string Name { get; set; }
        public Category ParentCategory { get; set; }

        [JsonIgnore]
        public List<Category> SubCategories { get; set; } = new List<Category>();

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
            if (parent != null) parent.SubCategories.Add(this);
        }

        public bool RemoveSubCategory(Category sub)
        {
            if (sub.SubCategories.Count == 0)
            {
                return SubCategories.Remove(sub);
            }
            return false;
        }

        public override string ToString()
        {
            return Name;
        }

        public List<Product> GetProducts(List<Product> allProducts)
        {
            return allProducts.Where(p => p.Category == this).ToList();
        }
    }
}