using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartWarehouse
{
    public class ProductRepository : IRepository<Product>
    {
        private List<Product> _products;
        private readonly JsonDataService<Product> _dataService;

        public ProductRepository()
        {
            _dataService = new JsonDataService<Product>("products.json");
            var loaded = _dataService.Load();
            _products = loaded ?? new List<Product>();
        }

        public void Add(Product product)
        {
            _products.Add(product);
            SaveChanges();
        }

        public void Remove(Product product)
        {
            _products.Remove(product);
            SaveChanges();
        }

        public void SaveChanges()
        {
            _dataService.Save(_products);
        }

        public IEnumerable<Product> GetAll() => _products;

        public IEnumerable<Product> Find(Func<Product, bool> predicate) => _products.Where(predicate);
    }
}