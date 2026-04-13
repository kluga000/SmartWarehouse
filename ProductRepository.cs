using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartWarehouse
{
    public class ProductRepository : IRepository<Product>
    {
        private List<Product> _products = new List<Product>();

        public void Add(Product product)
        {
            _products.Add(product);
        }

        public void Remove(Product product)
        {
            _products.Remove(product);
        }

        public IEnumerable<Product> GetAll()
        {
            return _products;
        }

        public IEnumerable<Product> Find(Func<Product, bool> predicate)
        {
            return _products.Where(predicate);
        }
    }
}