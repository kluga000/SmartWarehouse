using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace SmartWarehouse
{
    // Універсальний сервіс для збереження та завантаження даних у форматі JSON
    // Використовує Generics <T> для роботи з будь-яким типом даних
    public class JsonDataService<T>
    {
        private readonly string _filePath;

        public JsonDataService(string fileName)
        {
            // Формуємо шлях до файлу в папці з запущеною програмою
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
        }

        // Збереження списку об'єктів у файл
        public void Save(IEnumerable<T> data)
        {
            // Налаштування для коректної роботи з посиланнями об'єктів
            var settings = new JsonSerializerSettings
            {
                // Ігноруємо циклічні посилання (важливо для ієрархії категорій)
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                // Форматування з відступами, щоб JSON був читабельним для людини
                Formatting = Formatting.Indented
            };

            // Перетворення об'єктів C# у текстовий формат JSON
            string json = JsonConvert.SerializeObject(data, settings);
            File.WriteAllText(_filePath, json);
        }

        // Завантаження даних з файлу
        public List<T> Load()
        {
            // Якщо файлу ще немає (перший запуск), повертаємо порожній список
            if (!File.Exists(_filePath)) return new List<T>();

            try
            {
                string json = File.ReadAllText(_filePath);
                // Перетворення тексту назад у реальні об'єкти C#
                return JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
            }
            catch (Exception)
            {
                // У разі пошкодження файлу або помилки читання повертаємо чистий список
                return new List<T>();
            }
        }
    }
}