using System;
using System.Collections.Generic;

namespace SmartWarehouse
{
    // Універсальний інтерфейс для реалізації патерну Repository.
    // Визначає стандартний набір методів для роботи з будь-якими даними системи.
    public interface IRepository<T>
    {
        void Add(T item);

        void Remove(T item);

        IEnumerable<T> GetAll();

        IEnumerable<T> Find(Func<T, bool> predicate);

        // Примусове збереження поточного стану даних у JSON
        void SaveChanges();
    }
}