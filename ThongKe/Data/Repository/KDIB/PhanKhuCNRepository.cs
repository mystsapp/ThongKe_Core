using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ThongKe.Data.Models_KDIB;

namespace ThongKe.Data.Repository.KDIB
{
    public interface IPhanKhuCNRepository
    {
        Task<IEnumerable<PhanKhuCns>> FindIncludeOneAsync(Expression<Func<PhanKhuCns, object>> value1, Expression<Func<PhanKhuCns, bool>> value2);
        
    }
    public class PhanKhuCNRepository : IPhanKhuCNRepository
    {
        private readonly SaleDoanIBContext _context;

        public PhanKhuCNRepository(SaleDoanIBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PhanKhuCns>> FindIncludeOneAsync(Expression<Func<PhanKhuCns, object>> value1, Expression<Func<PhanKhuCns, bool>> value2)
        {
            return await _context.Set<PhanKhuCns>().Include(value1).Where(value2).ToListAsync();
        }
    }
}
