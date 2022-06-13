using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThongKe.Data.Models;
using ThongKe.Data.Models_QLTour;
using ThongKe.Data.Repository.QLTour;

namespace ThongKe.Data.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository userRepository { get; }
        IChiNhanhRepository chiNhanhRepository { get; }
        IDMDaiLyRepository dMDaiLyRepository { get; }
        IThongKeRepository thongKeRepository { get; }

        // qltour
        IPhongBanRepository phongBanRepository { get; }

        Task<int> Complete();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly thongkeContext _context;
        private readonly qltourContext _qltourContext;

        public UnitOfWork(thongkeContext context, qltourContext qltourContext)
        {
            _context = context;
            _qltourContext = qltourContext;

            userRepository = new UserRepository(_context);
            chiNhanhRepository = new ChiNhanhRepository(_context);
            dMDaiLyRepository = new DMDaiLyRepository(_context); 
            thongKeRepository = new ThongKeRepository(_context);

            // qltour
            phongBanRepository = new PhongBanRepository(_qltourContext);
        }

        public IUserRepository userRepository { get; }

        public IChiNhanhRepository chiNhanhRepository { get; }

        public IDMDaiLyRepository dMDaiLyRepository { get; }

        public IThongKeRepository thongKeRepository { get; }

        public IPhongBanRepository phongBanRepository { get; }

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
