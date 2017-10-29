using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq.Expressions;

namespace VOffice.Repository.Infrastructure.Contract
{
    public interface IBaseRepository<T>
    {
        T GetById(object primaryKey);
        IEnumerable<T> GetAll();
        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate);
        T Add(T entity);
        void Delete(T entity);
        T Edit(T entity);
        void Save();
        void DeleteMulti(Expression<Func<T, bool>> where);
    }
}