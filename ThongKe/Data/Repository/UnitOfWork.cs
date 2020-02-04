using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThongKe.Data.Models;

namespace ThongKe.Data.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        IAccountRepository accountRepository { get; }
        IChiNhanhRepository chiNhanhRepository { get; }
        IDMDaiLyRepository dMDaiLyRepository { get; }
        IThongKeRepository thongKeRepository { get; }
        Task<int> Complete();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly thongkeContext _context;

        public UnitOfWork(thongkeContext context)
        {
            _context = context;
            accountRepository = new AccountRepository(_context);
            chiNhanhRepository = new ChiNhanhRepository(_context);
            dMDaiLyRepository = new DMDaiLyRepository(_context); 
            thongKeRepository = new ThongKeRepository(_context);
        }

        public IAccountRepository accountRepository { get; }

        public IChiNhanhRepository chiNhanhRepository { get; }

        public IDMDaiLyRepository dMDaiLyRepository { get; }

        public IThongKeRepository thongKeRepository { get; }

        public async Task<int> Complete()
        {
            var a = await _context.SaveChangesAsync();
            return a;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
