using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ThongKe.Data.Interfaces;
using ThongKe.Data.Models;
using ThongKe.Data.Models_KDIB;
using ThongKe.Data.Models_KDND;
using ThongKe.Data.Models_KDOB;

namespace ThongKe.Data.Repository.KDIB
{
    public class Repository_KDOB<T> : IRepository<T> where T : class
    {
        protected readonly qlkdtrContext _context;

        public Repository_KDOB(qlkdtrContext context)
        {
            _context = context;
        }

        //protected void Save() => _context.SaveChanges();
        public int Count(Func<T, bool> predicate)
        {
            return _context.Set<T>().Where(predicate).Count();
        }

        public void Create(T entity)
        {
            _context.Add(entity);
            // Save();
        }

        public void Delete(T entity)
        {
            _context.Remove(entity);
            // Save();
        }

        public IEnumerable<T> Find(Func<T, bool> predicate)
        {
            return _context.Set<T>().Where(predicate);
        }

        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>();
        }

        public T GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }

        public void Update(T entity)
        {
            _context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

        }

        public async Task<T> GetByIdAsync(int? id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task Save() => await _context.SaveChangesAsync();

        public T GetSingleNoTracking(Func<T, bool> predicate)
        {
            return _context.Set<T>().AsNoTracking().SingleOrDefault(predicate);
        }

        public async Task<IEnumerable<T>> GetAllIncludeAsync(Expression<Func<T, object>> predicate, Expression<Func<T, object>> predicate2)
        {
            return await _context.Set<T>().Include(predicate).Include(predicate2).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllIncludeOneAsync(Expression<Func<T, object>> expression)
        {
            return await _context.Set<T>().Include(expression).ToListAsync();
        }

        public T GetById(string id)
        {
            return _context.Set<T>().Find(id);
        }

        public async Task<T> GetByIdAsync(string id)
        {
            return await _context.Set<T>().FindAsync(id);
        }
    }
}
