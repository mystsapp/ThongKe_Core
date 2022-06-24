using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThongKe.Data.Models;
using ThongKe.Data.Models_KDIB;
using ThongKe.Data.Models_KDND;
using ThongKe.Data.Models_KDOB;
using ThongKe.Data.Models_QLTour;
using ThongKe.Data.Repository.KDIB;
using ThongKe.Data.Repository.KDND;
using ThongKe.Data.Repository.KDOB;
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
        IUserIBRepository userIBRepository { get; }

        // KDND
        ITourKDNDRepository tourKDNDRepository { get; }

        // KDND
        ITourKDOBRepository tourKDOBRepository { get; }

        Task<int> Complete();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly thongkeContext _context;
        private readonly qltourContext _qltourContext;
        private readonly SaleDoanIBContext _saleDoanIBContext;
        private readonly qlkdtrnoidiaContext _qlkdtrnoidiaContext;
        private readonly qlkdtrContext _qlkdtrContext;

        public UnitOfWork(thongkeContext context, qltourContext qltourContext, SaleDoanIBContext saleDoanIBContext,
            qlkdtrnoidiaContext qlkdtrnoidiaContext, qlkdtrContext qlkdtrContext)
        {
            _context = context;
            _qltourContext = qltourContext;
            _saleDoanIBContext = saleDoanIBContext;
            _qlkdtrnoidiaContext = qlkdtrnoidiaContext;
            _qlkdtrContext = qlkdtrContext;

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
            userIBRepository = new UserIBRepository(_saleDoanIBContext);

            // KDND
            tourKDNDRepository = new TourKDNDRepository(_qlkdtrnoidiaContext);

            // KDOB
            tourKDOBRepository = new TourKDOBRepository(_qlkdtrContext);
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

        public ITourKDNDRepository tourKDNDRepository { get; }

        public IUserIBRepository userIBRepository {get;}

        public ITourKDOBRepository tourKDOBRepository { get; }

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
