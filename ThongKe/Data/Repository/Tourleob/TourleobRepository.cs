using System;
using System.Collections.Generic;
using System.Linq;
using ThongKe.Data.DTOs.Tourleob;
using ThongKe.Data.Models_Tourleob;

namespace ThongKe.Data.Repository.Tourleob
{
    public interface ITourleobRepository
    {
        KhachVaVetourDTO GetKhachvetours(DateTime searchFromDate, DateTime searchToDate, string chinhanh, string quay);
        List<Dmdaily> GetDmdailys(List<string> chinhanhs);
    }

    public class TourleobRepository : ITourleobRepository
    {
        private readonly tourleobContext _context;

        public TourleobRepository(tourleobContext context)
        {
            _context = context;
        }

        /// <summary>
        /// y tuong: lay tat ca vetour trong khoang thoi gian, sau do lay tat ca khachvetour tu vetour do
        /// where: chinhanh theo tour hay chinhanh cua daily xuat ve????????????
        /// </summary>
        /// <param name="searchFromDate"></param>
        /// <param name="searchToDate"></param>
        /// <param name="chinhanh"></param>
        /// <param name="quay"></param>
        /// <returns></returns>
        public KhachVaVetourDTO GetKhachvetours(DateTime searchFromDate, DateTime searchToDate, 
                                                string chinhanh, string quay)
        {
            KhachVaVetourDTO khachVaVetourDTO = new KhachVaVetourDTO()
            {
                VetourDTOs = new List<VetourDTO>(),
                KhachvetourDTOs = new List<KhachvetourDTO>()
            };
            quay ??= "";
            List<VetourDTO> vetours = new List<VetourDTO>();
            vetours = (from u in _context.Vetour
                       join c in _context.Tour on u.Sgtcode equals c.Sgtcode into x
                       from e in x.DefaultIfEmpty()
                       where e.Huytour == null
                          && e.Khachle == true
                          && (e.Chinhanh == chinhanh || "" == chinhanh)
                             //&& (userAppRole == SD.Sale
                             //        ? (e.Nguoixuatve!.Trim() == SD.convertToUnSign3(hoten.Trim()).ToUpper())
                             //        : ("" == ""))
                             && (u.Dailyxuatve == quay || "" == quay)
                             && (u.Ngayxuatve >= searchFromDate && u.Ngayxuatve < searchToDate.AddDays(1))
                             && !string.IsNullOrEmpty(u.Dailyxuatve)
                       select new VetourDTO
                       {
                           Id = u.Id,
                           Sgtcode = u.Sgtcode,
                           VetourId = u.VetourId,
                           Ngaytao = u.Ngaytao,
                           Makh = u.Makh,
                           Tencoquan = u.Tencoquan,
                           Tenkhach = u.Tenkhach,
                           Diachi = u.Diachi,
                           Quan = u.Quan,
                           Thanhpho = u.Thanhpho,
                           Dienthoai = u.Dienthoai,
                           Email = u.Email,
                           Diemdon = u.Diemdon,
                           Ghichuvetour = u.Ghichuvetour,
                           Dichvukhac = u.Dichvukhac,
                           Ghichudvk = u.Ghichudvk,
                           Yeucauks = u.Yeucauks,
                           Serial = u.Serial,
                           Ngayxuatve = u.Ngayxuatve,
                           Nguoixuatve = u.Nguoixuatve,
                           Dailyxuatve = u.Dailyxuatve,
                           Giatour = u.Giatour,
                           Hoahong = u.Hoahong,
                           Ngaychihh = u.Ngaychihh,
                           Nguoinhanhh = u.Nguoinhanhh,
                           Phieuchihh = u.Phieuchihh,
                           Cachtinhhh = u.Cachtinhhh,
                           Nguoichihh = u.Nguoichihh,
                           Huyve = u.Huyve,
                           Ngayhuyve = u.Ngayhuyve,
                           Nguoihuyve = u.Nguoihuyve,
                           Dailyhuyve = u.Dailyhuyve,
                           Noidunghuyve = u.Noidunghuyve,
                           Idchuyenve = u.Idchuyenve,
                           Tienhoan = u.Tienhoan,
                           Lephihuy = u.Lephihuy,
                           Tuhuy = u.Tuhuy,
                           Kenhgd = u.Kenhgd,
                           Kenhtt = u.Kenhtt,
                           Magdonline = u.Magdonline,
                           Thetindung = u.Thetindung,
                           Giamgia = u.Giamgia,
                           Lydogiamgia = u.Lydogiamgia,
                           Chiemcho = u.Chiemcho,
                           TuyenTq = e.Tuyentq,
                           Chudetour = e.Chudetour,
                           DoanhThu = u.Giatour + u.Dichvukhac - u.Giamgia,
                           Batdau = e.Batdau,
                           Ketthuc = e.Ketthuc
                           //Chinhanh = e.Chinhanh // chinhanh xuat ve
                       }).Distinct().OrderByDescending(c => c.Sgtcode).ThenByDescending(c => c.Nguoixuatve).ToList();
            vetours.ForEach(vt =>
            {
                var dmdaily = _context.Dmdaily.Where(x => x.Trangthai == true && x.Daily.ToLower() == vt.Dailyxuatve.ToLower()).FirstOrDefault();
                vt.Chinhanh = (dmdaily == null) ? "" : dmdaily.Chinhanh;
            });
            #region du khachvetour
            //List<KhachvetourDTO> khachvetours = new List<KhachvetourDTO>();
            //khachvetours = (from u in _context.Vetour
            //                join c in _context.Khachvetour on u.Sgtcode equals c.Sgtcode into x
            //                from e in x.DefaultIfEmpty()
            //                    where u.VetourId == e.VetourId
            //                //      //&& (u.Chinhanh == chinhanh || "" == chinhanh)
            //                //      //&& (userAppRole == SD.Sale
            //                //      //        ? (e.Nguoixuatve!.Trim() == SD.convertToUnSign3(hoten.Trim()).ToUpper())
            //                //      //        : ("" == ""))
            //                //      && (u.Dailyxuatve == quay || "" == quay)
            //                      && !string.IsNullOrEmpty(u.Dailyxuatve)
            //                      && u.Ngayxuatve != null
            //                      && u.Ngayhuyve == null
            //                      && (u.Ngayxuatve >= searchFromDate && u.Ngayxuatve < searchToDate.AddDays(1))
            //                select new KhachvetourDTO()
            //                {
            //                    Idkhach = e.Idkhach,
            //                    Sgtcode = e.Sgtcode,
            //                    VetourId = e.VetourId,
            //                    Ngaytao = e.Ngaytao,
            //                    Makh = e.Makh,
            //                    Stt = e.Stt,
            //                    Tenkhach = e.Tenkhach,
            //                    Ngaysinh = e.Ngaysinh,
            //                    Hochieu = e.Hochieu,
            //                    Ngaycaphc = e.Ngaycaphc,
            //                    Hieuluchc = e.Hieuluchc,
            //                    Gioitinh = e.Gioitinh,
            //                    Diachi = e.Diachi,
            //                    Quan = e.Quan,
            //                    Thanhpho = e.Thanhpho,
            //                    Dienthoai = e.Dienthoai,
            //                    Email = e.Email,
            //                    Loaikhach = e.Loaikhach,
            //                    Dotuoi = e.Dotuoi,
            //                    Phongks = e.Phongks,
            //                    Ghichu = e.Ghichu,
            //                    Ghichuvisa = e.Ghichuvisa,
            //                    Ghichuvmb = e.Ghichuvmb,
            //                    Landtour = e.Landtour,
            //                    Airticket = e.Airticket,
            //                    Dichvukhac = e.Dichvukhac,
            //                    Ghichudvk = e.Ghichudvk,
            //                    Giamgia = e.Giamgia,
            //                    Ghichugg = e.Ghichugg,
            //                    Prn = e.Prn,
            //                    Hanxuatvmb = e.Hanxuatvmb,
            //                    Noixuatvmb = e.Noixuatvmb,
            //                    Hanhtrinh = e.Hanhtrinh,
            //                    Vanchuyen = e.Vanchuyen,
            //                    Doanhthunn = e.Doanhthunn,
            //                    Chiemcho = e.Chiemcho,
            //                    Quoctich = e.Quoctich,
            //                    Capnhat = e.Capnhat,
            //                    Computer = e.Computer,
            //                    Logfile = e.Logfile,
            //                    Huytour = e.Huytour,
            //                    Codegiamgia = e.Codegiamgia,
            //                    Idvetour = u.Id, // id vetour de lien ket
            //                    //// bonus properties
            //                    //TuyenTq = u.TuyenTq,
            //                    //Chudetour = u.Chudetour
            //                }).Distinct().OrderByDescending(c => c.Tenkhach).ToList();
            //khachVaVetourDTO.VetourDTOs = vetours;
            //khachvetours.ForEach(kv =>
            //{
            //    var vt = vetours.FirstOrDefault(v => v.Id == kv.Idvetour);
            //    if (vt != null)
            //    {
            //        kv.TuyenTq = vt.TuyenTq;
            //        kv.Chinhanh = vt.Chinhanh; // chinhanh xuat ve
            //    }
            //});
            //khachVaVetourDTO.KhachvetourDTOs = khachvetours;
            #endregion
            khachVaVetourDTO.VetourDTOs = vetours;
            return khachVaVetourDTO;
        }
        public List<Dmdaily> GetDmdailys(List<string> chinhanhs)
        {
            var dmdailies = _context.Dmdaily.Where(x => x.Trangthai == true).ToList();
            return (chinhanhs.Count == 0) ? dmdailies : dmdailies.Where(x => chinhanhs.Contains(x.Chinhanh)).ToList();
        }   
    }
}
