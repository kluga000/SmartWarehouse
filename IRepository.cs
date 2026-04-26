using System;
using System.Collections.Generic;

namespace SmartWarehouse
{
    public interface IRepository<T>
    {
        void Add(T item);
        void Remove(T item);
        IEnumerable<T> GetAll();
        IEnumerable<T> Find(Func<T, bool> predicate);
        void SaveChanges();
    }
}