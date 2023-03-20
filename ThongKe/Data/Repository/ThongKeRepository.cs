using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThongKe.Data.Interfaces;
using ThongKe.Data.Models;
using ThongKe.Models;

namespace ThongKe.Data.Repository
{
    public interface IThongKeRepository : IRepository<DoanThuDoanNgayDi>
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
        IEnumerable<DoanThuDoanNgayDi> listQuayTheoNgayBan(string tungay, string denngay, string chinhanh, string khoi);

        IEnumerable<DoanhthuQuayChitiet> QuayTheoNgayBanChiTietToExcel(string tungay, string denngay, string quay, string chinhanh, string khoi);
        

        /////////////////// Quay theo ngay di /////////////////////////////////////////////////
        IEnumerable<DoanThuDoanNgayDi> listQuayTheoNgayDi(string tungay, string denngay, string chinhanh, string khoi);

        IEnumerable<DoanhthuQuayChitiet> QuayTheoNgayDiChiTietToExcel(string tungay, string denngay, string quay, string chinhanh, string khoi);

        /////////////////// Doan theo ngay di /////////////////////////////////////////////////
        IEnumerable<DoanhthuDoanNgayDi> listDoanTheoNgayDi(string tungay, string denngay, string chinhanh, string khoi);

        IEnumerable<DoanhthuDoanChitiet> DoanTheoNgayDiChiTietToExcel(string sgtcode, string khoi);

        ////////////////////////////////////// Tuyentq Theo Ngay di //////////////////////////////////////////
        IEnumerable<TuyentqNgaydi> listTuyentqTheoNgayDi(string tungay, string denngay, string chinhanh, string khoi);

        IEnumerable<TuyentqChiTietViewModel> TuyentqTheoNgayDiChiTietToExcel(string tungay, string denngay, string chinhanh, string tuyentq, string khoi);


        ////////////////////////////////////// listTuyentqTheoNgayBan //////////////////////////////////////////
        IEnumerable<TuyenTqTheoNgayBan> listTuyentqTheoNgayBan(string tungay, string denngay, string chinhanh, string khoi);
        IEnumerable<TuyentqChiTietNgayBanViewModel> TuyentqTheoNgayBanChiTietToExcel(string tungay, string denngay, string chinhanh, string tuyentq, string khoi);

        ////////////////////////////////////// Tuyentq theo quy //////////////////////////////////////////
        IEnumerable<Tuyentheoquy> TuyenTqTheoQuyToExcel(int quy, int nam, string chinhanh, string khoi);

        ////////////////////////////////////// Khach le he thong //////////////////////////////////////////
        IEnumerable<DoanhthuToanhethong> listKhachLeHeThong(string tungay, string denngay, string chinhanh, string khoi);

        ////////////////////////////////////// Khach Huy //////////////////////////////////////////
        IEnumerable<KhachHuy> listKhachHuy(string tungay, string denngay, string chinhanh, string khoi);

        ////////////////////////////////////// Thong ke web //////////////////////////////////////////
        IEnumerable<Thongkeweb> listThongKeWeb(string tungay, string denngay, string khoi);

        IEnumerable<Thongkewebchitiet> ThongKeWebChiTietToExcel(string tungay, string denngay, string chinhanh, string khoi);

        /////////////////////////////// Thong ke web ngay di //////////////////////////////////////////////////
        IEnumerable<Thongkeweb> listThongKeWebNgayDi(string tungay, string denngay, string khoi);

        IEnumerable<Thongkewebchitiet> ThongKeWebNgayDiToExcel(string tungay, string denngay, string chinhanh, string khoi);

        /// ///////////////////////////////////////Home Chart///////////////////////////////////////////////////////////////////////////////
        IEnumerable<ThongKeKhachViewModel> ThongKeSoKhachOB(string khoi);

        IEnumerable<ThongKeDoanhThuViewModel> ThongKeDoanhThuOB(string khoi);

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        List<TourBySGTCodeViewModel> getTourbySgtcode(string sgtcode, string khoi);
    }

    public class ThongKeRepository : Repository<DoanThuDoanNgayDi>, IThongKeRepository
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
            tuyentq = tuyentq ?? "";
            if (tungay == null)
                return null;
            var parameter = new SqlParameter[]
              {
                    new SqlParameter("@tungay",tungay),
                    new SqlParameter("@denngay",denngay),
                    new SqlParameter("@tuyentq",tuyentq.Trim()),
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
                    new SqlParameter("@tungay",tungay),
                    new SqlParameter("@denngay",denngay),
                    new SqlParameter("@nhanvien",nhanvien),
                    new SqlParameter("@tuyentq",tuyentq),
                    new SqlParameter("@khoi",khoi)
              };
            var d = _context.DoanhthuSaleTuyentqChitiet.FromSqlRaw("EXECUTE dbo.spDoanhThuSaleChitietTuyentq @tungay, @denngay, @nhanvien, @tuyentq, @khoi", parameter).ToList();
            var count = d.Count();
            return d;
        }

        ////////////////////////////////////// Quay Theo Ngay Ban //////////////////////////////////////////
        public IEnumerable<DoanThuDoanNgayDi> listQuayTheoNgayBan(string tungay, string denngay, string chinhanh, string khoi)
        {
            if (tungay == null)
                return null;
            IEnumerable<DoanThuDoanNgayDi> d = null;
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

        public IEnumerable<DoanhthuQuayChitiet> QuayTheoNgayBanChiTietToExcel(string tungay, string denngay, string quay, string chinhanh, string khoi)
        {
            if (tungay == null)
                return null;
            var parameter = new SqlParameter[]
              {
                    new SqlParameter("@tungay",DateTime.Parse(tungay)),
                    new SqlParameter("@denngay",DateTime.Parse(denngay)),
                    new SqlParameter("@quay",quay),
                    new SqlParameter("@chinhanh",chinhanh),
                    new SqlParameter("@khoi",khoi)
              };
            var d = _context.DoanhthuQuayChitiet.FromSqlRaw("EXECUTE dbo.spDoanhSoQuayChitietNgayBan @tungay, @denngay, @quay, @chinhanh, @khoi", parameter).ToList();
            var count = d.Count();
            return d;
        }
        
        ////////////////////////////////////// Quay Theo Ngay di //////////////////////////////////////////
        ///
        public IEnumerable<DoanThuDoanNgayDi> listQuayTheoNgayDi(string tungay, string denngay, string chinhanh, string khoi)
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
            var d = _context.DoanthuQuayNgayBan.FromSqlRaw("EXECUTE dbo.spBaocaoDoanhThuQuayTheoNgayDi @tungay, @denngay, @chinhanh, @khoi", parameter).ToList();
            var count = d.Count();
            return d;
        }

        public IEnumerable<DoanhthuQuayChitiet> QuayTheoNgayDiChiTietToExcel(string tungay, string denngay, string quay, string chinhanh, string khoi)
        {
            if (tungay == null)
                return null;
            var parameter = new SqlParameter[]
              {
                   new SqlParameter("@quay",quay),
                    new SqlParameter("@chinhanh",chinhanh),
                    new SqlParameter("@tungay",DateTime.Parse(tungay)),
                    new SqlParameter("@denngay",DateTime.Parse(denngay)),
                    new SqlParameter("@khoi",khoi)
              };
            var d = _context.DoanhthuQuayChitiet.FromSqlRaw("EXECUTE dbo.spDoanhSoQuayChitietNgaydi @quay, @chinhanh, @tungay, @denngay, @khoi", parameter).ToList();
            var count = d.Count();
            return d;
        }

        ////////////////////////////////////// Doan Theo Ngay di //////////////////////////////////////////
        public IEnumerable<DoanhthuDoanNgayDi> listDoanTheoNgayDi(string tungay, string denngay, string chinhanh, string khoi)
        {
            khoi = khoi ?? "";
            if (tungay == null)
                return null;
            var parameter = new SqlParameter[]
              {
                    new SqlParameter("@tungay",tungay),
                    new SqlParameter("@denngay",denngay),
                    new SqlParameter("@chinhanh",chinhanh),
                    new SqlParameter("@khoi",khoi)
              };
            var d = _context.DoanhthuDoanNgayDi.FromSqlRaw("EXECUTE dbo.spBaocaoDoanhThuDoanTheoNgayDi @tungay, @denngay, @chinhanh, @khoi", parameter).ToList();
            var count = d.Count();
            return d;
        }

        public IEnumerable<DoanhthuDoanChitiet> DoanTheoNgayDiChiTietToExcel(string sgtcode, string khoi)
        {
            var parameter = new SqlParameter[]
             {
                    new SqlParameter("@sgtcode",sgtcode),
                    new SqlParameter("@khoi",khoi)
             };
            var result = _context.DoanhthuDoanChitiet.FromSqlRaw("EXECUTE dbo.spDoanhthuDoanChitiet @sgtcode, @khoi", parameter).ToList();
            var count = result.Count();

            return result;
        }

        ////////////////////////////////////// Tuyentq Theo Ngay di //////////////////////////////////////////
        public IEnumerable<TuyentqNgaydi> listTuyentqTheoNgayDi(string tungay, string denngay, string chinhanh, string khoi)
        {
            if (tungay == null)
                return null;
            IEnumerable<TuyentqNgaydi> d = null;
            var parameter = new SqlParameter[]
              {
                    new SqlParameter("@tungay",DateTime.Parse(tungay)),
                    new SqlParameter("@denngay",DateTime.Parse(denngay)),
                    new SqlParameter("@chinhanh",chinhanh)
              };
            if (khoi == "OB")
            {
                d = _context.TuyentqNgaydi.FromSqlRaw("EXECUTE dbo.spThongkeTuyentqTheoNgayDiOB @tungay, @denngay, @chinhanh", parameter).ToList();
            }
            else
            {
                d = _context.TuyentqNgaydi.FromSqlRaw("EXECUTE dbo.spThongkeTuyentqTheoNgayDiND @tungay, @denngay, @chinhanh", parameter).ToList();
            }
            var count = d.Count();
            return d;
        }

        public IEnumerable<TuyentqChiTietViewModel> TuyentqTheoNgayDiChiTietToExcel(string tungay, string denngay, string chinhanh, string tuyentq, string khoi)
        {
            if (tungay == null)
                return null;
            IEnumerable<TuyentqChiTietViewModel> d = null;
            var parameter = new SqlParameter[]
              {
                    new SqlParameter("@tungay",tungay),
                    new SqlParameter("@denngay",denngay),
                    new SqlParameter("@chinhanh",chinhanh),
                    new SqlParameter("@tuyentq",tuyentq)
              };
            if (khoi == "OB")
            {
                d = _context.TuyentqChiTietViewModels.FromSqlRaw("EXECUTE dbo.spThongkeTuyentqChitietOB @tungay, @denngay, @chinhanh, @tuyentq", parameter).ToList();
            }
            else
            {
                d = _context.TuyentqChiTietViewModels.FromSqlRaw("EXECUTE dbo.spThongkeTuyentqChitietND @tungay, @denngay, @chinhanh, @tuyentq", parameter).ToList();
            }
            var count = d.Count();
            return d;
        }

        ////////////////////////////////////// listTuyentqTheoNgayBan //////////////////////////////////////////
        public IEnumerable<TuyenTqTheoNgayBan> listTuyentqTheoNgayBan(string tungay, string denngay, string chinhanh, string khoi)
        {
            if (tungay == null)
                return null;
            IEnumerable<TuyenTqTheoNgayBan> d = null;
            var parameter = new SqlParameter[]
              {
                    new SqlParameter("@tungay",DateTime.Parse(tungay)),
                    new SqlParameter("@denngay",DateTime.Parse(denngay)),
                    new SqlParameter("@chinhanh",chinhanh)
              };
            if (khoi == "OB") 
            {
                d = _context.TuyenTqTheoNgayBans.FromSqlRaw("EXECUTE dbo.spThongkeTuyentqTheoNgayBanOB @tungay, @denngay, @chinhanh", parameter).ToList();
            }
            else
            {
                d = _context.TuyenTqTheoNgayBans.FromSqlRaw("EXECUTE dbo.spThongkeTuyentqTheoNgayBanND @tungay, @denngay, @chinhanh", parameter).ToList();
            }
            var count = d.Count();
            return d;
        }

        public IEnumerable<TuyentqChiTietNgayBanViewModel> TuyentqTheoNgayBanChiTietToExcel(string tungay, string denngay, string chinhanh, string tuyentq, string khoi)
        {
            if (tungay == null)
                return null;
            IEnumerable<TuyentqChiTietNgayBanViewModel> d = null;
            
            var parameter = new SqlParameter[]
              {
                    new SqlParameter("@tungay", Convert.ToDateTime(tungay)),
                    new SqlParameter("@denngay", Convert.ToDateTime(denngay)),
                    new SqlParameter("@chinhanh",chinhanh),
                    new SqlParameter("@tuyentq",tuyentq)
              };
            try
            {
                if (khoi == "OB")
                {
                    d = _context.TuyentqChiTietNgayBanViewModels.FromSqlRaw("EXECUTE dbo.spThongkeTuyentqChitietOB_NgayBan @tungay, @denngay, @chinhanh, @tuyentq", parameter).ToList();
                }
                else
                {
                    d = _context.TuyentqChiTietNgayBanViewModels.FromSqlRaw("EXECUTE dbo.spThongkeTuyentqChitietND_NgayBan @tungay, @denngay, @chinhanh, @tuyentq", parameter).ToList();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
            var count = d.Count();
            return d;
        }

        ////////////////////////////////////// Tuyentq theo quy //////////////////////////////////////////
        public IEnumerable<Tuyentheoquy> TuyenTqTheoQuyToExcel(int quy, int nam, string chinhanh, string khoi)
        {
            var parameter = new SqlParameter[]
              {
                    new SqlParameter("@quy", quy),
                    new SqlParameter("@nam", nam),
                    new SqlParameter("@chinhanh",chinhanh),
                    new SqlParameter("@khoi",khoi)
              };
            var d = _context.Tuyentheoquy.FromSqlRaw("EXECUTE dbo.spThongkeTuyenTheoQuy @quy, @nam, @chinhanh, @khoi", parameter).ToList();
            var count = d.Count();
            return d;
        }

        ////////////////////////////////////// Khach le he thong //////////////////////////////////////////
        public IEnumerable<DoanhthuToanhethong> listKhachLeHeThong(string tungay, string denngay, string chinhanh, string khoi)
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

            var d = _context.DoanhthuToanhethong.FromSqlRaw("EXECUTE dbo.spThongkeKhachToanHeThong @tungay, @denngay, @chinhanh, @khoi", parameter).ToList();

            var count = d.Count();
            return d;
        }

        /////////////////////////////// Khach Huy /////////////////////
        public IEnumerable<KhachHuy> listKhachHuy(string tungay, string denngay, string chinhanh, string khoi)
        {
            if (tungay == null)
                return null;
            var parameter = new SqlParameter[]
              {
                        new SqlParameter("@tungay", tungay),
                        new SqlParameter("@denngay",denngay),
                        new SqlParameter("@chinhanh", chinhanh),
                        new SqlParameter("@khoi",khoi)
              };
            var d = _context.KhachHuys.FromSqlRaw("EXECUTE dbo.spThongKehuydoan @tungay, @denngay, @chinhanh, @khoi", parameter).ToList();
            var count = d.Count();
            return d;
        }

        /////////////////////////////// Thong ke web //////////////////////////////////////////////////
        public IEnumerable<Thongkeweb> listThongKeWeb(string tungay, string denngay, string khoi)
        {
            if (tungay == null)
                return null;
            var parameter = new SqlParameter[]
              {
                        new SqlParameter("@tungay", Convert.ToDateTime(tungay)),
                        new SqlParameter("@denngay",Convert.ToDateTime(denngay)),
                        new SqlParameter("@khoi",khoi)
              };
            var d = _context.Thongkeweb.FromSqlRaw("EXECUTE dbo.spThongkeWeb @tungay, @denngay, @khoi", parameter).ToList();
            var count = d.Count();
            return d;
        }

        public IEnumerable<Thongkewebchitiet> ThongKeWebChiTietToExcel(string tungay, string denngay, string chinhanh, string khoi)
        {
            if (tungay == null)
                return null;

            IEnumerable<Thongkewebchitiet> d = null;
            var parameter = new SqlParameter[]
              {
                        new SqlParameter("@tungay", Convert.ToDateTime(tungay)),
                        new SqlParameter("@denngay",Convert.ToDateTime(denngay)),
                        new SqlParameter("@chinhanh",chinhanh)
              };
            if (khoi == "OB")
            {
                d = _context.Thongkewebchitiet.FromSqlRaw("EXECUTE dbo.spThongkeWebchitietOB @tungay, @denngay, @chinhanh", parameter).ToList();
            }
            else
            {
                d = _context.Thongkewebchitiet.FromSqlRaw("EXECUTE dbo.spThongkeWebchitietND @tungay, @denngay, @chinhanh", parameter).ToList();
            }

            var count = d.Count();
            return d;
        }

        /////////////////////////////// Thong ke web ngay di //////////////////////////////////////////////////
        public IEnumerable<Thongkeweb> listThongKeWebNgayDi(string tungay, string denngay, string khoi)
        {
            if (tungay == null)
                return null;
            var parameter = new SqlParameter[]
              {
                        new SqlParameter("@tungay", Convert.ToDateTime(tungay)),
                        new SqlParameter("@denngay",Convert.ToDateTime(denngay)),
                        new SqlParameter("@khoi",khoi)
              };
            var d = _context.Thongkeweb.FromSqlRaw("EXECUTE dbo.spThongkeWeb_ngaydi @tungay, @denngay, @khoi", parameter).ToList();
            var count = d.Count();
            return d;
        }

        public IEnumerable<Thongkewebchitiet> ThongKeWebNgayDiToExcel(string tungay, string denngay, string chinhanh, string khoi)
        {
            if (tungay == null)
                return null;

            IEnumerable<Thongkewebchitiet> d = null;
            var parameter = new SqlParameter[]
              {
                        new SqlParameter("@tungay", Convert.ToDateTime(tungay)),
                        new SqlParameter("@denngay",Convert.ToDateTime(denngay)),
                        new SqlParameter("@chinhanh",chinhanh)
              };
            if (khoi == "OB")
            {
                d = _context.Thongkewebchitiet.FromSqlRaw("EXECUTE dbo.spThongkeWebchitiet_ngaydiOB @tungay, @denngay, @chinhanh", parameter).ToList();
            }
            else
            {
                d = _context.Thongkewebchitiet.FromSqlRaw("EXECUTE dbo.spThongkeWebchitiet_ngaydiND @tungay, @denngay, @chinhanh", parameter).ToList();
            }

            var count = d.Count();
            return d;
        }

        /// ///////////////////////////////////////Home Chart///////////////////////////////////////////////////////////////////////////////
        public IEnumerable<ThongKeKhachViewModel> ThongKeSoKhachOB(string khoi)
        {
            var parameter = new SqlParameter[]
              {
                        new SqlParameter("@khoi",khoi)
              };
            var result = _context.ThongKeKhachViewModels.FromSqlRaw("EXECUTE dbo.spThongkeKhach @khoi", parameter).ToList();
            return result;
        }

        public IEnumerable<ThongKeDoanhThuViewModel> ThongKeDoanhThuOB(string khoi)
        {
            var parameter = new SqlParameter[]
            {
                new SqlParameter("@khoi",khoi)
            };
            var result = _context.ThongKeDoanhThuViewModels.FromSqlRaw("EXECUTE dbo.spThongKeDoanhthu @khoi", parameter).ToList();
            return result;
        }

        /// /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public List<TourBySGTCodeViewModel> getTourbySgtcode(string sgtcode, string khoi)
        {
            try
            {
                //var result = DbContext.spGetTourByCode(sgtcode, khoi).ToList();
                var parameter = new SqlParameter[]
             {
                    new SqlParameter("@sgtcode",sgtcode),
                    new SqlParameter("@khoi",khoi)
             };
                var result = _context.TourBySGTCodeViewModels.FromSqlRaw("EXECUTE dbo.spGetTourByCode @sgtcode, @khoi", parameter).ToList();
                var count = result.Count();

                return result;
            }
            catch
            {
                throw;
            }
        }
    }
}