using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThongKe.Data.Models_QLTour;

namespace ThongKe.Data.Repository.QLTour
{
    public interface IDmChiNhanhRepository
    {
        IEnumerable<Dmchinhanh> GetAll();

        Task<Dmchinhanh> GetByIdAsync(int id);
        Dmchinhanh GetById(int id);

        IEnumerable<Dmchinhanh> Find(Func<Dmchinhanh, bool> predicate);
        //IPagedList<Dmchinhanh> ListChiNhanh(string searchString, int? page);
    }
    public class DmChiNhanhRepository : IDmChiNhanhRepository
    {
        private readonly qltourContext _qltourContext;

        public DmChiNhanhRepository(qltourContext qltourContext)
        {
            _qltourContext = qltourContext;
        }

        public IEnumerable<Dmchinhanh> Find(Func<Dmchinhanh, bool> predicate)
        {
            return _qltourContext.Dmchinhanh.Where(predicate);
        }

        public IEnumerable<Dmchinhanh> GetAll()
        {
            return _qltourContext.Dmchinhanh;
        }

        public Dmchinhanh GetById(int id)
        {
            return _qltourContext.Dmchinhanh.Find(id);
        }

        public async Task<Dmchinhanh> GetByIdAsync(int id)
        {
            return await _qltourContext.Dmchinhanh.FindAsync(id);
        }

        //public IPagedList<Dmchinhanh> ListChiNhanh(string searchString, int? page)
        //{
        //    // return a 404 if user browses to before the first page
        //    if (page.HasValue && page < 1)
        //        return null;

        //    // retrieve list from database/whereverand

        //    var list = GetAll().AsQueryable();
        //    if (!string.IsNullOrEmpty(searchString))
        //    {
        //        list = list.Where(x => x.Tencn.ToLower().Contains(searchString.ToLower()) ||
        //                               x.Macn.ToLower().Contains(searchString.ToLower()) ||
        //                               x.Thanhpho.ToLower().Contains(searchString.ToLower()) ||
        //                               x.Diachi.ToLower().Contains(searchString.ToLower()));
        //    }

        //    var count = list.Count();

        //    // page the list
        //    const int pageSize = 15;
        //    decimal aa = (decimal)list.Count() / (decimal)pageSize;
        //    var bb = Math.Ceiling(aa);
        //    if (page > bb)
        //    {
        //        page--;
        //    }
        //    page = (page == 0) ? 1 : page;
        //    var listPaged = list.ToPagedList(page ?? 1, pageSize);
        //    //if (page > listPaged.PageCount)
        //    //    page--;
        //    // return a 404 if user browses to pages beyond last page. special case first page if no items exist
        //    if (listPaged.PageNumber != 1 && page.HasValue && page > listPaged.PageCount)
        //        return null;


        //    return listPaged;


        //}
    }
}
