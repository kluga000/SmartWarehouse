using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartWarehouse
{
    // Репозиторій для управління колекцією товарів
    // Реалізує патерн збереження даних через JSON-сервіс
    public class ProductRepository : IRepository<Product>
    {
        // Внутрішній список для оперативної роботи з даними в пам'яті
        private List<Product> _products;

        // Сервіс для серіалізації/десеріалізації даних у файл
        private readonly JsonDataService<Product> _dataService;

        public ProductRepository()
        {
            // Ініціалізація сервісу з вказанням імені файлу бази даних
            _dataService = new JsonDataService<Product>("products.json");

            // Завантаження даних при старті програми. Якщо файл порожній — створюємо новий список.
            var loaded = _dataService.Load();
            _products = loaded ?? new List<Product>();
        }

        // Додавання об'єкта в колекцію з негайним збереженням змін
        public void Add(Product product)
        {
            _products.Add(product);
            SaveChanges();
        }

        // Повне видалення об'єкта з колекції (і відповідно з JSON-файлу)
        public void Remove(Product product)
        {
            _products.Remove(product);
            SaveChanges();
        }

        // Синхронізація поточного стану списку _products із фізичним файлом
        public void SaveChanges()
        {
            _dataService.Save(_products);
        }

        // Повернення всієї колекції для відображення в DataGrid
        public IEnumerable<Product> GetAll() => _products;

        // Гнучкий пошук за будь-яким критерієм (назва, артикул тощо) через LINQ-вирази
        public IEnumerable<Product> Find(Func<Product, bool> predicate) => _products.Where(predicate);
    }
}