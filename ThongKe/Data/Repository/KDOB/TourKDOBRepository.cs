using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ThongKe.Data.Interfaces;
using ThongKe.Data.Models_KDOB;

namespace ThongKe.Data.Repository.KDOB
{

    public interface ITourKDOBRepository
    {
        //IPagedList<TourDto> ListTour(string searchString, /*IEnumerable<Company> companies,*/ IEnumerable<Tourkind> loaiTours, IEnumerable<Dmchinhanh> chiNhanhs, IEnumerable<CacNoiDungHuyTour> cacNoiDungHuyTours, int? page, string searchFromDate, string searchToDate, List<string> listRoleChiNhanh, List<string> userInPhongBanQL);
        IEnumerable<Tour> Find(Func<Tour, bool> value);
    }

    public class TourKDOBRepository : ITourKDOBRepository
    {
        private readonly qlkdtrContext _context;

        public TourKDOBRepository(qlkdtrContext context)
        {
            _context = context;
        }

        public IEnumerable<Tour> Find(Func<Tour, bool> value)
        {
            return _context.Tour.Where(value);
        }

        //public IPagedList<TourDto> ListTour(string searchString,
        //                                    //IEnumerable<Company> companies,
        //                                    IEnumerable<Tourkind> loaiTours,
        //                                    IEnumerable<Dmchinhanh> chiNhanhs,
        //                                    IEnumerable<CacNoiDungHuyTour> cacNoiDungHuyTours,
        //                                    int? page,
        //                                    string searchFromDate, // ngay bat dau
        //                                    string searchToDate, // ngay ket thuc
        //                                    List<string> listRoleChiNhanh,
        //                                    List<string> userInPhongBanQL)
        //{
        //    // return a 404 if user browses to before the first page
        //    if (page.HasValue && page < 1)
        //        return null;

        //    // retrieve list from database/whereverand

        //    var list = new List<TourDto>();
        //    var tours = _context.Tours.ToList();
        //    if (userInPhongBanQL.Count() > 0) // role khac' admins va users
        //    {
        //        tours = tours.Where(item1 => userInPhongBanQL.Any(item2 => item1.NguoiTao == item2)).ToList();
        //    }

        //    if (tours == null)
        //    {
        //        return null;
        //    }
        //    foreach (var item in tours)
        //    {
        //        var tourDto = new TourDto();

        //        tourDto.Id = item.Id;
        //        tourDto.Sgtcode = item.Sgtcode;
        //        tourDto.KhachLe = item.KhachLe;
        //        tourDto.ChuDeTour = item.ChuDeTour;
        //        tourDto.ThiTruong = item.PhongDH;
        //        tourDto.NgayKhoa = item.NgayKhoa;
        //        tourDto.NguoiKhoa = item.NguoiKhoa;
        //        tourDto.NgayTao = item.NgayTao;
        //        tourDto.NguoiTao = item.NguoiTao;
        //        tourDto.NgayDen = item.NgayDen;
        //        tourDto.NgayDi = item.NgayDi;
        //        tourDto.TuyenTQ = item.TuyenTQ;
        //        tourDto.SoKhachDK = item.SoKhachDK;
        //        tourDto.DoanhThuDK = item.DoanhThuDK;
        //        //tourDto.CompanyName = companies.Where(x => x.CompanyId == item.MaKH).FirstOrDefault().Name;
        //        if (item.NgayDamPhan.HasValue)
        //        {
        //            tourDto.NgayDamPhan = item.NgayDamPhan.Value;
        //        }

        //        tourDto.HinhThucGiaoDich = item.HinhThucGiaoDich;
        //        if (item.NgayKyHopDong.HasValue)
        //        {
        //            tourDto.NgayKyHopDong = item.NgayKyHopDong.Value;
        //        }

        //        tourDto.NguoiKyHopDong = item.NguoiKyHopDong;
        //        if (item.HanXuatVe.HasValue)
        //        {
        //            tourDto.HanXuatVe = item.HanXuatVe.Value;
        //        }
        //        if (item.NgayThanhLyHD.HasValue)
        //        {
        //            tourDto.NgayThanhLyHD = item.NgayThanhLyHD.Value;
        //        }

        //        tourDto.SoKhachTT = item.SoKhachTT;
        //        tourDto.SKTreEm = item.SKTreEm;
        //        tourDto.DoanhThuTT = item.DoanhThuTT;
        //        tourDto.ChuongTrinhTour = item.ChuongTrinhTour;
        //        tourDto.NoiDungThanhLyHD = item.NoiDungThanhLyHD;
        //        tourDto.DichVu = item.DichVu;
        //        tourDto.DaiLy = item.DaiLy;
        //        tourDto.TrangThai = item.TrangThai;
        //        tourDto.NgaySua = item.NgaySua;
        //        tourDto.NguoiSua = item.NguoiSua;
        //        tourDto.TenLoaiTour = loaiTours.Where(x => x.Id == item.LoaiTourId).FirstOrDefault().TourkindInf;
        //        tourDto.MaCNTao = (item.ChiNhanhTaoId == 0) ? "" : chiNhanhs.Where(x => x.Id == item.ChiNhanhTaoId).FirstOrDefault().Macn;
        //        if (item.NgayNhanDuTien.HasValue)
        //        {
        //            tourDto.NgayNhanDuTien = item.NgayNhanDuTien.Value;
        //        }

        //        tourDto.LyDoNhanDu = item.LyDoNhanDu;
        //        tourDto.SoHopDong = item.SoHopDong;
        //        tourDto.LaiChuaVe = item.LaiChuaVe;
        //        tourDto.LaiGomVe = item.LaiGomVe;
        //        tourDto.LaiThucTeGomVe = item.LaiThucTeGomVe;
        //        tourDto.NguonTour = item.NguonTour;
        //        tourDto.FileKhachDiTour = item.FileKhachDiTour;
        //        tourDto.FileVeMayBay = item.FileVeMayBay;
        //        tourDto.FileBienNhan = item.FileBienNhan;
        //        tourDto.NguoiDaiDien = item.NguoiDaiDien;
        //        tourDto.DoiTacNuocNgoai = item.DoiTacNuocNgoai;
        //        tourDto.MaCNDH = chiNhanhs.Where(x => x.Id == item.ChiNhanhDHId).FirstOrDefault().Macn;
        //        if (item.NgayHuyTour.HasValue)
        //        {
        //            tourDto.NgayHuyTour = item.NgayHuyTour.Value;
        //        }
        //        tourDto.HuyTour = item.HuyTour;
        //        tourDto.NDHuyTour = (item.NDHuyTourId == 0) ? "" : cacNoiDungHuyTours.Where(x => x.Id == item.NDHuyTourId).FirstOrDefault().NoiDung;
        //        tourDto.GhiChu = item.GhiChu;
        //        tourDto.LoaiTien = item.LoaiTien;
        //        tourDto.TyGia = item.TyGia;
        //        tourDto.LogFile = item.LogFile;

        //        tourDto.Invoices = _context.Invoices.Where(x => x.TourId == item.Id).Count();

        //        list.Add(tourDto);
        //    }

        //    if (!string.IsNullOrEmpty(searchString))
        //    {
        //        list = list.Where(x => x.Sgtcode.ToLower().Contains(searchString.Trim().ToLower()) ||
        //                               (!string.IsNullOrEmpty(x.SoHopDong.ToLower()) && x.SoHopDong.ToLower().Contains(searchString.ToLower())) ||
        //                               (!string.IsNullOrEmpty(x.ChuDeTour.ToLower()) && x.ChuDeTour.ToLower().Contains(searchString.ToLower())) ||
        //                               (!string.IsNullOrEmpty(x.TuyenTQ.ToLower()) && x.TuyenTQ.ToLower().Contains(searchString.ToLower())) ||
        //                               (!string.IsNullOrEmpty(x.NguoiTao.ToLower()) && x.NguoiTao.ToLower().Contains(searchString.ToLower()))).ToList();
        //    }
        //    list = list.OrderByDescending(x => x.NgayTao).ToList();
        //    var count = list.Count();

        //    // search date
        //    DateTime fromDate, toDate;
        //    if (!string.IsNullOrEmpty(searchFromDate) && !string.IsNullOrEmpty(searchToDate))
        //    {
        //        try
        //        {
        //            fromDate = DateTime.Parse(searchFromDate); // ngay bat dau
        //            toDate = DateTime.Parse(searchToDate); // ngay ket thuc

        //            if (fromDate > toDate)
        //            {
        //                return null;
        //            }
        //            //list = list.Where(x => x.NgayTao >= fromDate &&
        //            //                   x.NgayTao < toDate.AddDays(1)).ToList();
        //            list = list.Where(x => x.NgayDen >= fromDate &&
        //                               x.NgayDi < toDate.AddDays(1)).ToList();
        //        }
        //        catch (Exception)
        //        {
        //            return null;
        //        }
        //    }
        //    else
        //    {
        //        if (!string.IsNullOrEmpty(searchFromDate)) // ngay bat dau
        //        {
        //            try
        //            {
        //                fromDate = DateTime.Parse(searchFromDate);
        //                list = list.Where(x => x.NgayDen >= fromDate).ToList();
        //            }
        //            catch (Exception)
        //            {
        //                return null;
        //            }
        //        }
        //        if (!string.IsNullOrEmpty(searchToDate)) // ngay ket thuc
        //        {
        //            try
        //            {
        //                toDate = DateTime.Parse(searchToDate);
        //                list = list.Where(x => x.NgayDi < toDate.AddDays(1)).ToList();
        //            }
        //            catch (Exception)
        //            {
        //                return null;
        //            }
        //        }
        //    }
        //    // search date

        //    // List<string> listRoleChiNhanh --> chi lay nhung tour thuộc phanKhuCN cua minh
        //    if (listRoleChiNhanh.Count > 0)
        //    {
        //        list = list.Where(item1 => listRoleChiNhanh.Any(item2 => item1.MaCNTao == item2)).ToList();
        //    }
        //    // List<string> listRoleChiNhanh --> chi lay nhung tour thuộc phanKhuCN cua minh

        //    // page the list
        //    const int pageSize = 10;
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
