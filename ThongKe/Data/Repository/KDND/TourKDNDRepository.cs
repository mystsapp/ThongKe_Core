using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ThongKe.Data.Interfaces;
using ThongKe.Data.Models_KDND;

namespace ThongKe.Data.Repository.KDND
{

    public interface ITourKDNDRepository
    {
        //IPagedList<TourDto> ListTour(string searchString, /*IEnumerable<Company> companies,*/ IEnumerable<Tourkind> loaiTours, IEnumerable<Dmchinhanh> chiNhanhs, IEnumerable<CacNoiDungHuyTour> cacNoiDungHuyTours, int? page, string searchFromDate, string searchToDate, List<string> listRoleChiNhanh, List<string> userInPhongBanQL);
        IEnumerable<Tour> Find(Func<Tour, bool> value);
        IEnumerable<Loaitour> GetLoaitours();
    }

    public class TourKDNDRepository : ITourKDNDRepository
    {
        private readonly qlkdtrnoidiaContext _context;

        public TourKDNDRepository(qlkdtrnoidiaContext context)
        {
            _context = context;
        }

        public IEnumerable<Tour> Find(Func<Tour, bool> value)
        {
            return _context.Tour.Where(value);
        }

        
        public IEnumerable<Loaitour> GetLoaitours()
        {
            return _context.Loaitour;
        }


    }
}
