using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThongKe.Data.Interfaces;
using ThongKe.Data.Models;

namespace ThongKe.Data.Repository
{
    public interface IThongKeRepository : IRepository<DoanthuQuayNgayBan>
    {
        ////////////////// sale theo QUAY //////////////////////////////
        IEnumerable<DoanhthuSaleQuay> listSaleTheoQuay(string tungay, string denngay, string chinhanh, string khoi);
        IEnumerable<DoanhthuSaleChitiet> SaleTheoQuayChiTietToExcel(string tungay, string denngay, string nhanvien, string chinhanh, string khoi);
        ////////////////// sale theo ngay di //////////////////////////////
        IEnumerable<DoanhthuSaleQuay> ListSaleTheoNgayDi(string tungay, string denngay, string chinhanh, string khoi);
        IEnumerable<DoanhthuSaleQuay> SaleTheoNgayDiPost(string tungay, string denngay, string chinhanh, string khoi);
        IEnumerable<DoanhthuSaleChitiet> SaleTheoNgayDiChiTietToExcel(string tungay, string denngay, string nhanvien, string chinhanh, string khoi);
        /////////////////// sale theo tuyen tham quan /////////////////////////////////////////
        IEnumerable<DoanhthuSaleTuyen> ListSaleTheoTuyenThamQuan(string tungay, string denngay, string tuyentq, string khoi);
        IEnumerable<DoanhthuSaleTuyentqChitiet> SaleTheoTuyenThamQuanChiTietToExcel(string tungay, string denngay, string nhanvien, string tuyentq, string khoi);

        /////////////////// Quay theo ngay ban /////////////////////////////////////////////////
        IEnumerable<DoanthuQuayNgayBan> listQuayTheoNgayBan(string tungay, string denngay, string chinhanh, string khoi);

    }
    public class ThongKeRepository : Repository<DoanthuQuayNgayBan>, IThongKeRepository
    {
        public ThongKeRepository(thongkeContext context) : base(context)
        {
        }

        /////////////////////////////// sale theo quay /////////////////////
        public IEnumerable<DoanhthuSaleQuay> listSaleTheoQuay(string tungay, string denngay, string chinhanh, string khoi)
        {
            if (tungay == null)
                return null;
            var parameter = new SqlParameter[]
              {
                    new SqlParameter("@tungay",DateTime.Parse(tungay)),
                    new SqlParameter("@denngay",DateTime.Parse(denngay)),
                    new SqlParameter("@chinhanh",chinhanh),
                    new SqlParameter("@khoi",khoi)
              };
            var d = _context.DoanhthuSaleQuay.FromSqlRaw("EXECUTE dbo.spBaocaoDoanhThuSaleTheoQuay @tungay, @denngay, @chinhanh, @khoi", parameter).ToList();
            var count = d.Count();
            return d;
        }

        public IEnumerable<DoanhthuSaleChitiet> SaleTheoQuayChiTietToExcel(string tungay, string denngay, string nhanvien, string chinhanh, string khoi)
        {
            if (tungay == null)
                return null;
            var parameter = new SqlParameter[]
              {
                    new SqlParameter("@tungay",DateTime.Parse(tungay)),
                    new SqlParameter("@denngay",DateTime.Parse(denngay)),
                    new SqlParameter("@nhanvien",nhanvien),
                    new SqlParameter("@chinhanh",chinhanh),
                    new SqlParameter("@khoi",khoi)
              };
            var d = _context.DoanhthuSaleChitiet.FromSqlRaw("EXECUTE dbo.spDoanhThuSaleChitietNgayban @tungay, @denngay, @nhanvien, @chinhanh, @khoi", parameter).ToList();
            var count = d.Count();
            return d;
        }

        /////////////////////////////// sale theo ngay di /////////////////////
        public IEnumerable<DoanhthuSaleQuay> ListSaleTheoNgayDi(string tungay, string denngay, string chinhanh, string khoi)
        {
            if (tungay == null)
                return null;
            var parameter = new SqlParameter[]
              {
                    new SqlParameter("@tungay",DateTime.Parse(tungay)),
                    new SqlParameter("@denngay",DateTime.Parse(denngay)),
                    new SqlParameter("@chinhanh",chinhanh),
                    new SqlParameter("@khoi",khoi)
              };
            var d = _context.DoanhthuSaleQuay.FromSqlRaw("EXECUTE dbo.spBaocaoDoanhThuSaleTheoNgayDi @tungay, @denngay, @chinhanh, @khoi", parameter).ToList();
            var count = d.Count();
            return d;
        }

        public IEnumerable<DoanhthuSaleQuay> SaleTheoNgayDiPost(string tungay, string denngay, string chinhanh, string khoi)
        {
            if (tungay == null)
                return null;
            var parameter = new SqlParameter[]
              {
                    new SqlParameter("@tungay",DateTime.Parse(tungay)),
                    new SqlParameter("@denngay",DateTime.Parse(denngay)),
                    new SqlParameter("@chinhanh",chinhanh),
                    new SqlParameter("@khoi",khoi)
              };
            var d = _context.DoanhthuSaleQuay.FromSqlRaw("EXECUTE dbo.spBaocaoDoanhThuSaleTheoNgayDi @tungay, @denngay, @chinhanh, @khoi", parameter).ToList();
            var count = d.Count();
            return d;
        }

        public IEnumerable<DoanhthuSaleChitiet> SaleTheoNgayDiChiTietToExcel(string tungay, string denngay, string nhanvien, string chinhanh, string khoi)
        {
            if (tungay == null)
                return null;
            var parameter = new SqlParameter[]
              {
                    new SqlParameter("@tungay",DateTime.Parse(tungay)),
                    new SqlParameter("@denngay",DateTime.Parse(denngay)),
                    new SqlParameter("@nhanvien",nhanvien),
                    new SqlParameter("@chinhanh",chinhanh),
                    new SqlParameter("@khoi",khoi)
              };
            var d = _context.DoanhthuSaleChitiet.FromSqlRaw("EXECUTE dbo.spDoanhThuSaleChitietNgaydi @tungay, @denngay, @nhanvien, @chinhanh, @khoi", parameter).ToList();
            var count = d.Count();
            return d;
        }

        ////////////////////////////////////// Sale theo tuyen tham quan //////////////////////////////////////////

        public IEnumerable<DoanhthuSaleTuyen> ListSaleTheoTuyenThamQuan(string tungay, string denngay, string tuyentq, string khoi)
        {
            if (tungay == null)
                return null;
            var parameter = new SqlParameter[]
              {
                    new SqlParameter("@tungay",DateTime.Parse(tungay)),
                    new SqlParameter("@denngay",DateTime.Parse(denngay)),
                    new SqlParameter("@tuyentq",tuyentq),
                    new SqlParameter("@khoi",khoi)
              };
            var d = _context.DoanhthuSaleTuyen.FromSqlRaw("EXECUTE dbo.spBaocaoDoanhThuSaleTheoTuyen @tungay, @denngay, @tuyentq, @khoi", parameter).ToList();
            var count = d.Count();
            return d;
        }
        public IEnumerable<DoanhthuSaleTuyentqChitiet> SaleTheoTuyenThamQuanChiTietToExcel(string tungay, string denngay, string nhanvien, string tuyentq, string khoi)
        {
            if (tungay == null)
                return null;
            var parameter = new SqlParameter[]
              {
                    new SqlParameter("@tungay",DateTime.Parse(tungay)),
                    new SqlParameter("@denngay",DateTime.Parse(denngay)),
                    new SqlParameter("@nhanvien",nhanvien),
                    new SqlParameter("@tuyentq",tuyentq),
                    new SqlParameter("@khoi",khoi)
              };
            var d = _context.DoanhthuSaleTuyentqChitiet.FromSqlRaw("EXECUTE dbo.spDoanhThuSaleChitietTuyentq @tungay, @denngay, @nhanvien, @tuyentq, @khoi", parameter).ToList();
            var count = d.Count();
            return d;
        }

        ////////////////////////////////////// Quay Theo Ngay Ban //////////////////////////////////////////
        public IEnumerable<DoanthuQuayNgayBan> listQuayTheoNgayBan(string tungay, string denngay, string chinhanh, string khoi)
        {
            if (tungay == null)
                return null;
            IEnumerable<DoanthuQuayNgayBan> d = null;
            var parameter = new SqlParameter[]
              {
                    new SqlParameter("@tungay",DateTime.Parse(tungay)),
                    new SqlParameter("@denngay",DateTime.Parse(denngay)),
                    new SqlParameter("@chinhanh",chinhanh)
              };
            if (khoi == "OB")
            {
                d = _context.DoanthuQuayNgayBan.FromSqlRaw("EXECUTE dbo.spBaocaoDoanhThuQuayTheoNgayBanOB @tungay, @denngay, @chinhanh", parameter).ToList();
            }
            else
            {
                d = _context.DoanthuQuayNgayBan.FromSqlRaw("EXECUTE dbo.spBaocaoDoanhThuQuayTheoNgayBanND @tungay, @denngay, @chinhanh", parameter).ToList();
            }
            var count = d.Count();
            return d;
        }
    }
}
