using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace SmartWarehouse
{
    public class JsonDataService<T>
    {
        private readonly string _filePath;

        public JsonDataService(string fileName)
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
        }

        public void Save(IEnumerable<T> data)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };

            string json = JsonConvert.SerializeObject(data, settings);
            File.WriteAllText(_filePath, json);
        }

        public List<T> Load()
        {
            if (!File.Exists(_filePath)) return new List<T>();

            try
            {
                string json = File.ReadAllText(_filePath);
                return JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
            }
            catch
            {
                return new List<T>();
            }
        }
    }
}