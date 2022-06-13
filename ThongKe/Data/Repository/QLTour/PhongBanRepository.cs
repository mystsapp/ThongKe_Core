using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThongKe.Data.Models_QLTour;

namespace ThongKe.Data.Repository.QLTour
{
    public interface IPhongBanRepository
    {
        IEnumerable<Phongban> GetAll();

        Task<Phongban> GetByIdAsync(int id);
        Task<Phongban> GetByIdAsync(string id);

        IEnumerable<Phongban> Find(Func<Phongban, bool> predicate);
        //IPagedList<Phongban> ListChiNhanh(string searchString, int? page);
    }
    public class PhongBanRepository : IPhongBanRepository
    {
        private readonly qltourContext _qltourContext;

        public PhongBanRepository(qltourContext qltourContext)
        {
            _qltourContext = qltourContext;
        }

        public IEnumerable<Phongban> Find(Func<Phongban, bool> predicate)
        {
            return _qltourContext.Phongban.Where(predicate);
        }

        public IEnumerable<Phongban> GetAll()
        {
            return _qltourContext.Phongban;
        }

        public async Task<Phongban> GetByIdAsync(int id)
        {
            return await _qltourContext.Phongban.FindAsync(id);
        }

        public async Task<Phongban> GetByIdAsync(string id)
        {
            return await _qltourContext.Phongban.FindAsync(id);
        }

        //public IPagedList<Phongban> ListChiNhanh(string searchString, int? page)
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
