using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ThongKe.Data.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        Task<IEnumerable<T>> GetAllIncludeOneAsync(Expression<Func<T, object>> expression);
        Task<IEnumerable<T>> GetAllIncludeAsync(Expression<Func<T, object>> predicate, Expression<Func<T, object>> predicate2);
        IEnumerable<T> Find(Func<T, bool> predicate);
        T GetById(int id);
        T GetSingleNoTracking(Func<T, bool> predicate);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
        int Count(Func<T, bool> predicate);

        Task Save();

        Task<T> GetByIdAsync(int? id);


        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    }
}
