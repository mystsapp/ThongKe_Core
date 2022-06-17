using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThongKe.Data.Models;
using ThongKe.Data.Models_KDIB;
using ThongKe.Data.Models_QLTour;
using ThongKe.Data.Repository.KDIB;
using ThongKe.Data.Repository.QLTour;

namespace ThongKe.Data.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        // thongke
        IUserRepository userRepository { get; }
        IChiNhanhRepository chiNhanhRepository { get; }
        IDMDaiLyRepository dMDaiLyRepository { get; }
        IThongKeRepository thongKeRepository { get; }
        IRoleRepository roleRepository { get; }

        // qltour
        IPhongBanRepository phongBanRepository { get; }
        IDmChiNhanhRepository dmChiNhanhRepository { get; }
        ITourKindRepository tourKindRepository { get; }
        ICompanyRepository companyRepository { get; }

        // KDIB
        ICacNoiDungHuyTourRepository cacNoiDungHuyTourRepository { get; }
        ITourKDIBRepository tourKDIBRepository { get; }
        IPhanKhuCNRepository phanKhuCNRepository { get; }

        Task<int> Complete();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly thongkeContext _context;
        private readonly qltourContext _qltourContext;
        private readonly SaleDoanIBContext _saleDoanIBContext;

        public UnitOfWork(thongkeContext context, qltourContext qltourContext, SaleDoanIBContext saleDoanIBContext)
        {
            _context = context;
            _qltourContext = qltourContext;
            _saleDoanIBContext = saleDoanIBContext;

            userRepository = new UserRepository(_context);
            chiNhanhRepository = new ChiNhanhRepository(_context);
            dMDaiLyRepository = new DMDaiLyRepository(_context); 
            thongKeRepository = new ThongKeRepository(_context);
            roleRepository = new RoleRepository(_context);

            // qltour
            phongBanRepository = new PhongBanRepository(_qltourContext);
            dmChiNhanhRepository = new DmChiNhanhRepository(_qltourContext);
            tourKindRepository = new TourKindRepository(_qltourContext);
            companyRepository = new CompanyRepository(_qltourContext);

            // KDIB
            cacNoiDungHuyTourRepository = new CacNoiDungHuyTourRepository(_saleDoanIBContext);
            tourKDIBRepository = new TourKDIBRepository(_saleDoanIBContext);
            phanKhuCNRepository = new PhanKhuCNRepository(_saleDoanIBContext);
        }

        public IUserRepository userRepository { get; }

        public IChiNhanhRepository chiNhanhRepository { get; }

        public IDMDaiLyRepository dMDaiLyRepository { get; }

        public IThongKeRepository thongKeRepository { get; }

        public IPhongBanRepository phongBanRepository { get; }

        public IDmChiNhanhRepository dmChiNhanhRepository { get; }

        public ITourKindRepository tourKindRepository { get; }

        public ICacNoiDungHuyTourRepository cacNoiDungHuyTourRepository { get; }

        public ITourKDIBRepository tourKDIBRepository { get; }

        public ICompanyRepository companyRepository { get; }

        public IPhanKhuCNRepository phanKhuCNRepository { get; }

        public IRoleRepository roleRepository { get; }

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
