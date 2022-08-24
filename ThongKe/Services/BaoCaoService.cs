﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThongKe.Data.DTOs;
using ThongKe.Data.Models;
using ThongKe.Data.Models_KDIB;
using ThongKe.Data.Models_QLTour;
using ThongKe.Data.Repository;
using ThongKe.Data.Models_KDND;

namespace ThongKe.Services
{
    public interface IBaoCaoService
    {
        IEnumerable<TourIBDTO> DoanhSoTheoThiTruong_IB(string searchFromDate, string searchToDate, Dmchinhanh dmchinhanh, List<string> thiTruongs, string username);
        //IEnumerable<TourIBDTO> DoanhSoTheoThiTruong_IB(string searchFromDate, string searchToDate, List<Dmchinhanh> dmchinhanhs, List<string> thiTruongs, string username);
        IEnumerable<TourIBDTO> DoanhSoTheoSale(string searchFromDate, string searchToDate, List<string> MaCNs);
        Task<IEnumerable<Role>> GetRoles();
        Task<Role> GetRoleById(int id);
        IEnumerable<Dmchinhanh> GetAllChiNhanh();
        IEnumerable<Company> GetCompanies();
        IEnumerable<TourNDDTO> DoanhSoTheoDaiLy(string searchFromDate, string searchToDate, List<string> daiLyQL);
        IEnumerable<TourNDDTO> DoanhSoTheoSale_ND(string searchFromDate, string searchToDate, string maCn, string username);
        IEnumerable<TourOBDTO> DoanhSoTheoSale_OB(string searchFromDate, string searchToDate, string maCn, string username);
        IEnumerable<TourIBDTO> DoanhSoTheoThang_IB(string searchFromDate, string searchToDate, Dmchinhanh chiNhanh, List<string> phongBans, string username);
        //IEnumerable<TourIBDTO> DoanhSoTheoThang_IB(string searchFromDate, string searchToDate, List<Dmchinhanh> chiNhanhs, List<string> phongBans, string username);
        IEnumerable<TourNDDTO> DoanhSoTheoThang_ND(string searchFromDate, string searchToDate, string chiNhanh, string username);
        //IEnumerable<TourNDDTO> DoanhSoTheoThang_ND(string searchFromDate, string searchToDate, List<string> chiNhanhs, string username);
        IEnumerable<TourOBDTO> DoanhSoTheoThang_OB(string searchFromDate, string searchToDate, string chiNhanh, string username);
        //IEnumerable<TourOBDTO> DoanhSoTheoThang_OB(string searchFromDate, string searchToDate, List<string> chiNhanhs, string username);
        IEnumerable<TourIBDTO> DoanhSoTheoNgay_IB(string searchFromDate, string searchToDate, string loaiTour, Dmchinhanh dmchinhanh, List<string> phongBanQLs, string username);
        IEnumerable<TourNDDTO> DoanhSoTheoNgay_ND(string searchFromDate, string searchToDate, string loaiTour, string maCn, string username);
        IEnumerable<TourOBDTO> DoanhSoTheoNgay_OB(string searchFromDate, string searchToDate, string loaiTour, string maCn, string username);
        IEnumerable<TourIBDTO> DoanhSoTheoSale_IB(string searchFromDate, string searchToDate, Dmchinhanh dmchinhanh, List<string> thiTruongs, string username);
        //IEnumerable<TourIBDTO> DoanhSoTheoSale_IB(string searchFromDate, string searchToDate, List<Dmchinhanh> dmchinhanhs, List<string> thiTruongs, string username);
        IEnumerable<Tourkind> GetTourinds();
        IEnumerable<Loaitour> GetLoaiTours();
        IEnumerable<DoanhthuSaleTuyen> ListSaleTheoTuyenThamQuan(string tungay, string denngay, string chiNhanh, string tuyentq, string khoi);

    }
    public class BaoCaoService : IBaoCaoService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BaoCaoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<TourNDDTO> DoanhSoTheoSale_ND(string searchFromDate, string searchToDate, string maCn, string username)
        {

            var list = new List<TourNDDTO>();
            var tours = new List<Data.Models_KDND.Tour>();
            //var companies = _unitOfWork.companyRepository.GetAll();
            var chiNhanhs = _unitOfWork.dmChiNhanhRepository.GetAll();
            //var loaiTours = _unitOfWork.tourKindRepository.GetAll();
            //var cacNoiDungHuyTours = _unitOfWork.cacNoiDungHuyTourRepository.GetAll();

            #region search date
            DateTime fromDate, toDate;
            if (!string.IsNullOrEmpty(searchFromDate) && !string.IsNullOrEmpty(searchToDate))
            {

                try
                {
                    fromDate = DateTime.Parse(searchFromDate);
                    toDate = DateTime.Parse(searchToDate);

                    if (fromDate > toDate)
                    {
                        return null;
                    }
                    tours = _unitOfWork.tourKDNDRepository.Find(x => x.Batdau >= fromDate &&
                                       x.Batdau < toDate.AddDays(1)).ToList();
                }
                catch (Exception)
                {

                    return null;
                }


                //list.Where(x => x.NgayTao >= fromDate && x.NgayTao < (toDate.AddDays(1))/*.ToPagedList(page, pageSize)*/;



            }
            else
            {
                if (!string.IsNullOrEmpty(searchFromDate))
                {
                    try
                    {
                        fromDate = DateTime.Parse(searchFromDate);
                        tours = _unitOfWork.tourKDNDRepository.Find(x => x.Batdau >= fromDate).ToList();
                    }
                    catch (Exception)
                    {
                        return null;
                    }

                }
                if (!string.IsNullOrEmpty(searchToDate))
                {
                    try
                    {
                        toDate = DateTime.Parse(searchToDate);
                        tours = _unitOfWork.tourKDNDRepository.Find(x => x.Batdau < toDate.AddDays(1)).ToList();

                    }
                    catch (Exception)
                    {
                        return null;
                    }

                }
            }
            #endregion

            if (tours == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(maCn))
            {
                tours = tours.Where(x => x.Chinhanh == maCn).ToList();
                if (!string.IsNullOrEmpty(username))
                {
                    tours = tours.Where(x => x.Nguoitao == username).ToList();
                }
            }
            foreach (var item in tours)
            {
                var tourDto = new TourNDDTO();

                tourDto.Idtour = item.Idtour;
                tourDto.Sgtcode = item.Sgtcode;
                tourDto.Chudetour = item.Chudetour;
                tourDto.Ngaytao = item.Ngaytao;
                tourDto.Nguoitao = item.Nguoitao;
                tourDto.Batdau = item.Batdau;
                tourDto.Ketthuc = item.Ketthuc;
                tourDto.Tuyentq = item.Tuyentq;
                tourDto.Diemtq = item.Diemtq;
                tourDto.Sokhachdk = item.Sokhachdk;
                tourDto.Sokhachtt = item.Sokhachtt;
                tourDto.Doanhthudk = item.Doanhthudk;
                tourDto.Doanhthutt = item.Doanhthutt;
                tourDto.Makh = item.Makh;
                tourDto.Tenkh = item.Tenkh;
                tourDto.Diachi = item.Diachi;
                tourDto.Dienthoai = item.Dienthoai;
                tourDto.Fax = item.Fax;
                tourDto.Email = item.Email;

                if (item.Ngaydamphan.HasValue)
                {
                    tourDto.Ngaydamphan = item.Ngaydamphan.Value;
                }

                tourDto.Hinhthucgiaodich = item.Hinhthucgiaodich;
                if (item.Ngaykyhopdong.HasValue)
                {
                    tourDto.Ngaykyhopdong = item.Ngaykyhopdong.Value;
                }

                tourDto.Nguoikyhopdong = item.Nguoikyhopdong;
                if (item.Hanxuatvmb.HasValue)
                {
                    tourDto.Hanxuatvmb = item.Hanxuatvmb.Value;
                }
                if (item.Ngaythanhlyhd.HasValue)
                {
                    tourDto.Ngaythanhlyhd = item.Ngaythanhlyhd.Value;
                }
                tourDto.Noidungthanhlyhd = item.Noidungthanhlyhd;
                tourDto.Dichvu = item.Dichvu;
                tourDto.Loaitourid = item.Loaitourid;
                tourDto.Trangthai = item.Trangthai;
                tourDto.Ngaysua = item.Ngaysua;
                tourDto.Nguoisua = item.Nguoisua;
                tourDto.Chinhanh = item.Chinhanh;
                tourDto.ChiNhanhDh = item.ChiNhanhDh;
                if (item.Ngaynhandutien.HasValue)
                {
                    tourDto.Ngaynhandutien = item.Ngaynhandutien.Value;
                }
                tourDto.Lidonhandu = item.Lidonhandu;
                tourDto.Sohopdong = item.Sohopdong;
                tourDto.Laichuave = item.Laichuave;
                tourDto.Laigomve = item.Laigomve;
                tourDto.Laithuctegomve = item.Laithuctegomve;
                tourDto.Nguyennhanhuythau = item.Nguyennhanhuythau;
                tourDto.Nguontour = item.Nguontour;
                tourDto.Filekhachditour = item.Filekhachditour;
                tourDto.Filevemaybay = item.Filevemaybay;
                tourDto.Filebiennhan = item.Filebiennhan;
                tourDto.Nguoidaidien = item.Nguoidaidien;
                tourDto.Doitacnuocngoai = item.Doitacnuocngoai;
                tourDto.Ngayhuytour = item.Ngayhuytour;
                tourDto.LogFile = item.LogFile;

                list.Add(tourDto);
            }

            list = list.Where(x => string.IsNullOrEmpty(x.Nguyennhanhuythau)).OrderByDescending(x => x.Batdau).ToList();
            var count = list.Count();

            return list;

        }

        public IEnumerable<TourOBDTO> DoanhSoTheoSale_OB(string searchFromDate, string searchToDate, string maCn, string username)
        {

            var list = new List<TourOBDTO>();
            var tours = new List<Data.Models_KDOB.Tour>();
            //var companies = _unitOfWork.companyRepository.GetAll();
            var chiNhanhs = _unitOfWork.dmChiNhanhRepository.GetAll();
            //var loaiTours = _unitOfWork.tourKindRepository.GetAll();
            //var cacNoiDungHuyTours = _unitOfWork.cacNoiDungHuyTourRepository.GetAll();

            #region search date
            // search date
            DateTime fromDate, toDate;
            if (!string.IsNullOrEmpty(searchFromDate) && !string.IsNullOrEmpty(searchToDate))
            {

                try
                {
                    fromDate = DateTime.Parse(searchFromDate);
                    toDate = DateTime.Parse(searchToDate);

                    if (fromDate > toDate)
                    {
                        return null;
                    }
                    tours = _unitOfWork.tourKDOBRepository.Find(x => x.Batdau >= fromDate &&
                                       x.Batdau < toDate.AddDays(1)).ToList();
                }
                catch (Exception)
                {

                    return null;
                }


                //list.Where(x => x.NgayTao >= fromDate && x.NgayTao < (toDate.AddDays(1))/*.ToPagedList(page, pageSize)*/;



            }
            else
            {
                if (!string.IsNullOrEmpty(searchFromDate))
                {
                    try
                    {
                        fromDate = DateTime.Parse(searchFromDate);
                        tours = _unitOfWork.tourKDOBRepository.Find(x => x.Batdau >= fromDate).ToList();
                    }
                    catch (Exception)
                    {
                        return null;
                    }

                }
                if (!string.IsNullOrEmpty(searchToDate))
                {
                    try
                    {
                        toDate = DateTime.Parse(searchToDate);
                        tours = _unitOfWork.tourKDOBRepository.Find(x => x.Batdau < toDate.AddDays(1)).ToList();

                    }
                    catch (Exception)
                    {
                        return null;
                    }

                }
            }
            // search date
            #endregion

            if (tours == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(maCn))
            {
                tours = tours.Where(x => x.Chinhanh == maCn).ToList();
                if (!string.IsNullOrEmpty(username))
                {
                    tours = tours.Where(x => x.Nguoitao == username).ToList();
                }
            }
            foreach (var item in tours)
            {
                var tourDto = new TourOBDTO();

                tourDto.Idtour = item.Idtour;
                tourDto.Sgtcode = item.Sgtcode;
                tourDto.Chudetour = item.Chudetour;
                tourDto.Ngaytao = item.Ngaytao;
                tourDto.Nguoitao = item.Nguoitao;
                tourDto.Batdau = item.Batdau;
                tourDto.Ketthuc = item.Ketthuc;
                tourDto.Tuyentq = item.Tuyentq;
                tourDto.Diemtq = item.Diemtq;
                tourDto.Sokhachdk = item.Sokhachdk;
                tourDto.Sokhachtt = item.Sokhachtt;
                tourDto.Doanhthudk = item.Doanhthudk;
                tourDto.Doanhthutt = item.Doanhthutt;
                tourDto.Makh = item.Makh;
                tourDto.Tenkh = item.Tenkh;
                tourDto.Diachi = item.Diachi;
                tourDto.Dienthoai = item.Dienthoai;
                tourDto.Fax = item.Fax;
                tourDto.Email = item.Email;

                if (item.Ngaydamphan.HasValue)
                {
                    tourDto.Ngaydamphan = item.Ngaydamphan.Value;
                }

                tourDto.Hinhthucgiaodich = item.Hinhthucgiaodich;
                if (item.Ngaykyhopdong.HasValue)
                {
                    tourDto.Ngaykyhopdong = item.Ngaykyhopdong.Value;
                }

                tourDto.Nguoikyhopdong = item.Nguoikyhopdong;
                if (item.Hanxuatvmb.HasValue)
                {
                    tourDto.Hanxuatvmb = item.Hanxuatvmb.Value;
                }
                if (item.Ngaythanhlyhd.HasValue)
                {
                    tourDto.Ngaythanhlyhd = item.Ngaythanhlyhd.Value;
                }
                tourDto.Noidungthanhlyhd = item.Noidungthanhlyhd;
                tourDto.Dichvu = item.Dichvu;
                tourDto.Loaitourid = item.Loaitourid;
                tourDto.Trangthai = item.Trangthai;
                tourDto.Ngaysua = item.Ngaysua;
                tourDto.Nguoisua = item.Nguoisua;
                tourDto.Chinhanh = item.Chinhanh;
                tourDto.ChiNhanhDh = item.ChiNhanhDh;
                if (item.Ngaynhandutien.HasValue)
                {
                    tourDto.Ngaynhandutien = item.Ngaynhandutien.Value;
                }
                tourDto.Lidonhandu = item.Lidonhandu;
                tourDto.Sohopdong = item.Sohopdong;
                tourDto.Laichuave = item.Laichuave;
                tourDto.Laigomve = item.Laigomve;
                tourDto.Laithuctegomve = item.Laithuctegomve;
                tourDto.Nguyennhanhuythau = item.Nguyennhanhuythau;
                tourDto.Nguontour = item.Nguontour;
                tourDto.Filekhachditour = item.Filekhachditour;
                tourDto.Filevemaybay = item.Filevemaybay;
                tourDto.Filebiennhan = item.Filebiennhan;
                tourDto.Nguoidaidien = item.Nguoidaidien;
                tourDto.Doitacnuocngoai = item.Doitacnuocngoai;
                tourDto.Ngayhuytour = item.Ngayhuytour;
                //tourDto.LogFile = item.LogFile;

                list.Add(tourDto);
            }

            list = list.Where(x => string.IsNullOrEmpty(x.Nguyennhanhuythau)).OrderByDescending(x => x.Batdau).ToList();
            var count = list.Count();

            return list;

        }

        public IEnumerable<TourNDDTO> DoanhSoTheoDaiLy(string searchFromDate, string searchToDate, List<string> daiLyQL)
        {
            var list = new List<TourNDDTO>();
            var tours = new List<Data.Models_KDND.Tour>();
            //var companies = _unitOfWork.companyRepository.GetAll();
            var chiNhanhs = _unitOfWork.dmChiNhanhRepository.GetAll();
            var loaiTours = _unitOfWork.tourKindRepository.GetAll();
            var cacNoiDungHuyTours = _unitOfWork.cacNoiDungHuyTourRepository.GetAll();

            // search date
            DateTime fromDate, toDate;
            if (!string.IsNullOrEmpty(searchFromDate) && !string.IsNullOrEmpty(searchToDate))
            {

                try
                {
                    fromDate = DateTime.Parse(searchFromDate);
                    toDate = DateTime.Parse(searchToDate);

                    if (fromDate > toDate)
                    {
                        return null;
                    }
                    tours = _unitOfWork.tourKDNDRepository.Find(x => x.Batdau >= fromDate &&
                                       x.Ketthuc < toDate.AddDays(1)).ToList();
                }
                catch (Exception)
                {

                    return null;
                }


                //list.Where(x => x.NgayTao >= fromDate && x.NgayTao < (toDate.AddDays(1))/*.ToPagedList(page, pageSize)*/;



            }
            else
            {
                if (!string.IsNullOrEmpty(searchFromDate))
                {
                    try
                    {
                        fromDate = DateTime.Parse(searchFromDate);
                        tours = _unitOfWork.tourKDNDRepository.Find(x => x.Batdau >= fromDate).ToList();
                    }
                    catch (Exception)
                    {
                        return null;
                    }

                }
                if (!string.IsNullOrEmpty(searchToDate))
                {
                    try
                    {
                        toDate = DateTime.Parse(searchToDate);
                        tours = _unitOfWork.tourKDNDRepository.Find(x => x.Ketthuc < toDate.AddDays(1)).ToList();

                    }
                    catch (Exception)
                    {
                        return null;
                    }

                }
            }
            // search date

            if (tours == null)
            {
                return null;
            }
            foreach (var item in tours)
            {
                var tourDto = new TourNDDTO();

                tourDto.Idtour = item.Idtour;
                tourDto.Sgtcode = item.Sgtcode;
                tourDto.Chudetour = item.Chudetour;
                tourDto.Ngaytao = item.Ngaytao;
                tourDto.Nguoitao = item.Nguoitao;
                tourDto.Batdau = item.Batdau;
                tourDto.Ketthuc = item.Ketthuc;
                tourDto.Tuyentq = item.Tuyentq;
                tourDto.Diemtq = item.Diemtq;
                tourDto.Sokhachdk = item.Sokhachdk;
                tourDto.Sokhachtt = item.Sokhachtt;
                tourDto.Doanhthudk = item.Doanhthudk;
                tourDto.Doanhthutt = item.Doanhthutt;
                tourDto.Makh = item.Makh;
                tourDto.Tenkh = item.Tenkh;
                tourDto.Diachi = item.Diachi;
                tourDto.Dienthoai = item.Dienthoai;
                tourDto.Fax = item.Fax;
                tourDto.Email = item.Email;

                if (item.Ngaydamphan.HasValue)
                {
                    tourDto.Ngaydamphan = item.Ngaydamphan.Value;
                }

                tourDto.Hinhthucgiaodich = item.Hinhthucgiaodich;
                if (item.Ngaykyhopdong.HasValue)
                {
                    tourDto.Ngaykyhopdong = item.Ngaykyhopdong.Value;
                }

                tourDto.Nguoikyhopdong = item.Nguoikyhopdong;
                if (item.Hanxuatvmb.HasValue)
                {
                    tourDto.Hanxuatvmb = item.Hanxuatvmb.Value;
                }
                if (item.Ngaythanhlyhd.HasValue)
                {
                    tourDto.Ngaythanhlyhd = item.Ngaythanhlyhd.Value;
                }
                tourDto.Noidungthanhlyhd = item.Noidungthanhlyhd;
                tourDto.Dichvu = item.Dichvu;
                tourDto.Loaitourid = item.Loaitourid;
                tourDto.Trangthai = item.Trangthai;
                tourDto.Ngaysua = item.Ngaysua;
                tourDto.Nguoisua = item.Nguoisua;
                tourDto.Chinhanh = item.Chinhanh;
                tourDto.ChiNhanhDh = item.ChiNhanhDh;
                if (item.Ngaynhandutien.HasValue)
                {
                    tourDto.Ngaynhandutien = item.Ngaynhandutien.Value;
                }
                tourDto.Lidonhandu = item.Lidonhandu;
                tourDto.Sohopdong = item.Sohopdong;
                tourDto.Laichuave = item.Laichuave;
                tourDto.Laigomve = item.Laigomve;
                tourDto.Laithuctegomve = item.Laithuctegomve;
                tourDto.Nguyennhanhuythau = item.Nguyennhanhuythau;
                tourDto.Nguontour = item.Nguontour;
                tourDto.Filekhachditour = item.Filekhachditour;
                tourDto.Filevemaybay = item.Filevemaybay;
                tourDto.Filebiennhan = item.Filebiennhan;
                tourDto.Nguoidaidien = item.Nguoidaidien;
                tourDto.Doitacnuocngoai = item.Doitacnuocngoai;
                tourDto.Ngayhuytour = item.Ngayhuytour;
                tourDto.LogFile = item.LogFile;

                list.Add(tourDto);
            }

            if (daiLyQL.Count > 0)
            {
                //list = list.Where(x => x.MaCNTao == macn).ToList();
                list = list.Where(item1 => daiLyQL.Any(item2 => item1.Daily == item2)).ToList();
            }
            list = list.Where(x => !string.IsNullOrEmpty(x.Nguyennhanhuythau)).OrderByDescending(x => x.Batdau).ToList();
            var count = list.Count();

            return list;
        }

        // DoanhSoTheoSale
        public IEnumerable<TourIBDTO> DoanhSoTheoSale(string searchFromDate, string searchToDate, List<string> MaCNs)
        {
            var list = new List<TourIBDTO>();
            var tours = new List<Data.Models_KDIB.Tours>();
            //var companies = _unitOfWork.companyRepository.GetAll();
            var chiNhanhs = _unitOfWork.dmChiNhanhRepository.GetAll();
            var loaiTours = _unitOfWork.tourKindRepository.GetAll();
            var cacNoiDungHuyTours = _unitOfWork.cacNoiDungHuyTourRepository.GetAll();

            #region search date
            DateTime fromDate, toDate;
            if (!string.IsNullOrEmpty(searchFromDate) && !string.IsNullOrEmpty(searchToDate))
            {

                try
                {
                    fromDate = DateTime.Parse(searchFromDate);
                    toDate = DateTime.Parse(searchToDate);

                    if (fromDate > toDate)
                    {
                        return null;
                    }
                    tours = _unitOfWork.tourKDIBRepository.Find(x => x.NgayDen >= fromDate &&
                                       x.NgayDi < toDate.AddDays(1)).ToList();
                }
                catch (Exception)
                {

                    return null;
                }


                //list.Where(x => x.NgayTao >= fromDate && x.NgayTao < (toDate.AddDays(1))/*.ToPagedList(page, pageSize)*/;



            }
            else
            {
                if (!string.IsNullOrEmpty(searchFromDate))
                {
                    try
                    {
                        fromDate = DateTime.Parse(searchFromDate);
                        tours = _unitOfWork.tourKDIBRepository.Find(x => x.NgayDen >= fromDate).ToList();
                    }
                    catch (Exception)
                    {
                        return null;
                    }

                }
                if (!string.IsNullOrEmpty(searchToDate))
                {
                    try
                    {
                        toDate = DateTime.Parse(searchToDate);
                        tours = _unitOfWork.tourKDIBRepository.Find(x => x.NgayDi < toDate.AddDays(1)).ToList();

                    }
                    catch (Exception)
                    {
                        return null;
                    }

                }
            }
            #endregion

            if (tours == null)
            {
                return null;
            }
            foreach (var item in tours)
            {
                var tourDto = new TourIBDTO();

                tourDto.Id = item.Id;
                tourDto.Sgtcode = item.Sgtcode;
                tourDto.KhachLe = item.KhachLe.Value;
                tourDto.ChuDeTour = item.ChuDeTour;
                tourDto.ThiTruong = item.PhongDh;
                tourDto.NgayKhoa = item.NgayKhoa;
                tourDto.NguoiKhoa = item.NguoiKhoa;
                tourDto.NgayTao = item.NgayTao;
                tourDto.NguoiTao = item.NguoiTao;
                tourDto.NgayDen = item.NgayDen;
                tourDto.NgayDi = item.NgayDi;
                tourDto.TuyenTQ = item.TuyenTq;
                tourDto.SoKhachDK = item.SoKhachDk;
                tourDto.DoanhThuDK = item.DoanhThuDk;
                tourDto.CompanyId = item.MaKh;// _unitOfWork.companyRepository.Find(x => x.CompanyId == item.MaKh).FirstOrDefault().Name;

                if (item.NgayDamPhan.HasValue)
                {
                    tourDto.NgayDamPhan = item.NgayDamPhan.Value;
                }

                tourDto.HinhThucGiaoDich = item.HinhThucGiaoDich;
                if (item.NgayKyHopDong.HasValue)
                {
                    tourDto.NgayKyHopDong = item.NgayKyHopDong.Value;
                }

                tourDto.NguoiKyHopDong = item.NguoiKyHopDong;
                if (item.HanXuatVe.HasValue)
                {
                    tourDto.HanXuatVe = item.HanXuatVe.Value;
                }
                if (item.NgayThanhLyHd.HasValue)
                {
                    tourDto.NgayThanhLyHD = item.NgayThanhLyHd.Value;
                }

                tourDto.SoKhachTT = item.SoKhachTt;
                tourDto.SKTreEm = item.SktreEm;
                tourDto.DoanhThuTT = item.DoanhThuTt;
                tourDto.ChuongTrinhTour = item.ChuongTrinhTour;
                tourDto.NoiDungThanhLyHD = item.NoiDungThanhLyHd;
                tourDto.DichVu = item.DichVu;
                tourDto.DaiLy = item.DaiLy;
                tourDto.TrangThai = item.TrangThai;
                tourDto.NgaySua = item.NgaySua;
                tourDto.NguoiSua = item.NguoiSua;
                tourDto.TenLoaiTour = loaiTours.Where(x => x.Id == item.LoaiTourId).FirstOrDefault().TourkindInf;
                tourDto.MaCNTao = (item.ChiNhanhTaoId == 0) ? "" : _unitOfWork.dmChiNhanhRepository.Find(x => x.Id == item.ChiNhanhTaoId).FirstOrDefault().Macn;// chiNhanhs.Where(x => x.Id == item.ChiNhanhTaoId).FirstOrDefault().Macn;
                if (item.NgayNhanDuTien.HasValue)
                {
                    tourDto.NgayNhanDuTien = item.NgayNhanDuTien.Value;
                }

                tourDto.LyDoNhanDu = item.LyDoNhanDu;
                tourDto.SoHopDong = item.SoHopDong;
                tourDto.LaiChuaVe = item.LaiChuaVe;
                tourDto.LaiGomVe = item.LaiGomVe;
                tourDto.LaiThucTeGomVe = item.LaiThucTeGomVe;
                tourDto.NguonTour = item.NguonTour;
                tourDto.FileKhachDiTour = item.FileKhachDiTour;
                tourDto.FileVeMayBay = item.FileVeMayBay;
                tourDto.FileBienNhan = item.FileBienNhan;
                tourDto.NguoiDaiDien = item.NguoiDaiDien;
                tourDto.DoiTacNuocNgoai = item.DoiTacNuocNgoai;
                tourDto.MaCNDH = chiNhanhs.Where(x => x.Id == item.ChiNhanhDhid).FirstOrDefault().Macn;
                if (item.NgayHuyTour.HasValue)
                {
                    tourDto.NgayHuyTour = item.NgayHuyTour.Value;
                }
                tourDto.HuyTour = item.HuyTour;
                tourDto.NDHuyTour = (item.NdhuyTourId == 0) ? "" : cacNoiDungHuyTours.Where(x => x.Id == item.NdhuyTourId).FirstOrDefault().NoiDung;
                tourDto.GhiChu = item.GhiChu;
                tourDto.LoaiTien = item.LoaiTien;
                tourDto.TyGia = item.TyGia;
                tourDto.LogFile = item.LogFile;

                //tourDto.Invoices = _context.Invoices.Where(x => x.TourId == item.Id).Count();

                list.Add(tourDto);
            }

            if (MaCNs.Count > 0)
            {
                //list = list.Where(x => x.MaCNTao == macn).ToList();
                list = list.Where(item1 => MaCNs.Any(item2 => item1.MaCNTao == item2)).ToList();
            }
            list = list.Where(x => x.HuyTour != true).OrderByDescending(x => x.NgayTao).ToList();
            var count = list.Count();

            return list;
        }

        // DoanhSoTheoThang_IB
        public IEnumerable<TourIBDTO> DoanhSoTheoThang_IB(string searchFromDate, string searchToDate, 
            Dmchinhanh chiNhanh, List<string> phongBanQLs, string username)
        {

            var tours = new List<Tours>();// _unitOfWork.tourRepository.Find(item1 => listCN.Any(item2 => item1.ChiNhanhTaoId == item2.Id)).ToList();
            #region search date
            // search date
            DateTime fromDate, toDate;
            if (!string.IsNullOrEmpty(searchFromDate) && !string.IsNullOrEmpty(searchToDate))
            {

                try
                {
                    fromDate = DateTime.Parse(searchFromDate);
                    toDate = DateTime.Parse(searchToDate);

                    if (fromDate > toDate)
                    {
                        return null;
                    }
                    tours = _unitOfWork.tourKDIBRepository.Find(x => x.NgayDen >= fromDate &&
                                       x.NgayDi < toDate.AddDays(1)).ToList();
                }
                catch (Exception)
                {

                    return null;
                }


                //list.Where(x => x.NgayTao >= fromDate && x.NgayTao < (toDate.AddDays(1))/*.ToPagedList(page, pageSize)*/;



            }
            else
            {
                if (!string.IsNullOrEmpty(searchFromDate))
                {
                    try
                    {
                        fromDate = DateTime.Parse(searchFromDate);
                        tours = _unitOfWork.tourKDIBRepository.Find(x => x.NgayDen >= fromDate).ToList();
                    }
                    catch (Exception)
                    {
                        return null;
                    }

                }
                if (!string.IsNullOrEmpty(searchToDate))
                {
                    try
                    {
                        toDate = DateTime.Parse(searchToDate);
                        tours = _unitOfWork.tourKDIBRepository.Find(x => x.NgayDi < toDate.AddDays(1)).ToList();

                    }
                    catch (Exception)
                    {
                        return null;
                    }

                }
            }
            // search date
            #endregion

            var list = new List<TourIBDTO>();
            //var companies = _unitOfWork.khachHangRepository.GetAll();
            var dmchinhanhs = _unitOfWork.dmChiNhanhRepository.GetAll();
            var loaiTours = _unitOfWork.tourKindRepository.GetAll();
            var cacNoiDungHuyTours = _unitOfWork.cacNoiDungHuyTourRepository.GetAll();

            if (tours == null)
            {
                return null;
            }
            else
            {
                if (chiNhanh != null) // chiNhanh
                {
                    tours = tours.Where(x => x.ChiNhanhTaoId == chiNhanh.Id).ToList();
                    //tours = tours.Where(item1 => chiNhanhs.Any(item2 => item1.ChiNhanhTaoId == item2.Id)).ToList();
                    if (phongBanQLs.Count > 0) // chi lay phong ban QL
                    {
                        var usernames = _unitOfWork.userRepository.Find(x => phongBanQLs.Any(y => y == x.PhongBanId)); // tat ca user trong thitruong
                        tours = tours.Where(x => usernames.Any(y => x.NguoiTao == y.Username)).ToList();

                    }
                    if (!string.IsNullOrEmpty(username)) // ko QL ai het ==> user thuong
                    {
                        tours = tours.Where(x => x.NguoiTao == username).ToList();
                    }
                }
            }

            foreach (var item in tours)
            {
                var tourDto = new TourIBDTO();

                tourDto.Id = item.Id;
                tourDto.Sgtcode = item.Sgtcode;
                tourDto.KhachLe = item.KhachLe.Value;
                tourDto.ChuDeTour = item.ChuDeTour;
                tourDto.ThiTruong = item.PhongDh;
                tourDto.NgayKhoa = item.NgayKhoa;
                tourDto.NguoiKhoa = item.NguoiKhoa;
                tourDto.NgayTao = item.NgayTao;
                tourDto.NguoiTao = item.NguoiTao;
                tourDto.NgayDen = item.NgayDen;
                tourDto.NgayDi = item.NgayDi;
                tourDto.TuyenTQ = item.TuyenTq;
                tourDto.SoKhachDK = item.SoKhachDk;
                tourDto.DoanhThuDK = item.DoanhThuDk * (item.TyGia ?? 1);
                tourDto.CompanyId = item.MaKh;// _unitOfWork.companyRepository.Find(x => x.CompanyId == item.MaKh).FirstOrDefault().Name;

                if (item.NgayDamPhan.HasValue)
                {
                    tourDto.NgayDamPhan = item.NgayDamPhan.Value;
                }

                tourDto.HinhThucGiaoDich = item.HinhThucGiaoDich;
                if (item.NgayKyHopDong.HasValue)
                {
                    tourDto.NgayKyHopDong = item.NgayKyHopDong.Value;
                }

                tourDto.NguoiKyHopDong = item.NguoiKyHopDong;
                if (item.HanXuatVe.HasValue)
                {
                    tourDto.HanXuatVe = item.HanXuatVe.Value;
                }
                if (item.NgayThanhLyHd.HasValue)
                {
                    tourDto.NgayThanhLyHD = item.NgayThanhLyHd.Value;
                }

                tourDto.SoKhachTT = item.SoKhachTt;
                tourDto.SKTreEm = item.SktreEm;
                tourDto.DoanhThuTT = item.DoanhThuTt * (item.TyGia ?? 1);
                tourDto.ChuongTrinhTour = item.ChuongTrinhTour;
                tourDto.NoiDungThanhLyHD = item.NoiDungThanhLyHd;
                tourDto.DichVu = item.DichVu;
                tourDto.DaiLy = item.DaiLy;
                tourDto.TrangThai = item.TrangThai;
                tourDto.NgaySua = item.NgaySua;
                tourDto.NguoiSua = item.NguoiSua;
                tourDto.TenLoaiTour = loaiTours.Where(x => x.Id == item.LoaiTourId).FirstOrDefault().TourkindInf;
                tourDto.MaCNTao = (item.ChiNhanhTaoId == 0) ? "" : _unitOfWork.dmChiNhanhRepository.Find(x => x.Id == item.ChiNhanhTaoId).FirstOrDefault().Macn;// chiNhanhs.Where(x => x.Id == item.ChiNhanhTaoId).FirstOrDefault().Macn;
                if (item.NgayNhanDuTien.HasValue)
                {
                    tourDto.NgayNhanDuTien = item.NgayNhanDuTien.Value;
                }

                tourDto.LyDoNhanDu = item.LyDoNhanDu;
                tourDto.SoHopDong = item.SoHopDong;
                tourDto.LaiChuaVe = item.LaiChuaVe;
                tourDto.LaiGomVe = item.LaiGomVe;
                tourDto.LaiThucTeGomVe = item.LaiThucTeGomVe;
                tourDto.NguonTour = item.NguonTour;
                tourDto.FileKhachDiTour = item.FileKhachDiTour;
                tourDto.FileVeMayBay = item.FileVeMayBay;
                tourDto.FileBienNhan = item.FileBienNhan;
                tourDto.NguoiDaiDien = item.NguoiDaiDien;
                tourDto.DoiTacNuocNgoai = item.DoiTacNuocNgoai;
                tourDto.MaCNDH = dmchinhanhs.Where(x => x.Id == item.ChiNhanhDhid).FirstOrDefault().Macn;
                if (item.NgayHuyTour.HasValue)
                {
                    tourDto.NgayHuyTour = item.NgayHuyTour.Value;
                }
                tourDto.HuyTour = item.HuyTour;
                tourDto.NDHuyTour = (item.NdhuyTourId == 0) ? "" : cacNoiDungHuyTours.Where(x => x.Id == item.NdhuyTourId).FirstOrDefault().NoiDung;
                tourDto.GhiChu = item.GhiChu;
                tourDto.LoaiTien = item.LoaiTien;
                tourDto.TyGia = item.TyGia;
                tourDto.LogFile = item.LogFile;

                //tourDto.Invoices = _context.Invoices.Where(x => x.TourId == item.Id).Count();

                list.Add(tourDto);
            }


            list = list.Where(x => x.HuyTour != true).OrderBy(x => x.NgayTao).ToList();
            var count = list.Count();

            return list;

        }
        
        // DoanhSoTheoThang_ND
        public IEnumerable<TourNDDTO> DoanhSoTheoThang_ND(string searchFromDate, string searchToDate, 
            string chiNhanh, string username)
        {

            var tours = new List<Data.Models_KDND.Tour>();// _unitOfWork.tourRepository.Find(item1 => listCN.Any(item2 => item1.ChiNhanhTaoId == item2.Id)).ToList();
            #region search date
            // search date
            DateTime fromDate, toDate;
            if (!string.IsNullOrEmpty(searchFromDate) && !string.IsNullOrEmpty(searchToDate))
            {

                try
                {
                    fromDate = DateTime.Parse(searchFromDate);
                    toDate = DateTime.Parse(searchToDate);

                    if (fromDate > toDate)
                    {
                        return null;
                    }
                    tours = _unitOfWork.tourKDNDRepository.Find(x => x.Batdau >= fromDate &&
                                       x.Batdau < toDate.AddDays(1)).ToList();
                }
                catch (Exception)
                {

                    return null;
                }


                //list.Where(x => x.NgayTao >= fromDate && x.NgayTao < (toDate.AddDays(1))/*.ToPagedList(page, pageSize)*/;



            }
            else
            {
                if (!string.IsNullOrEmpty(searchFromDate))
                {
                    try
                    {
                        fromDate = DateTime.Parse(searchFromDate);
                        tours = _unitOfWork.tourKDNDRepository.Find(x => x.Batdau >= fromDate).ToList();
                    }
                    catch (Exception)
                    {
                        return null;
                    }

                }
                if (!string.IsNullOrEmpty(searchToDate))
                {
                    try
                    {
                        toDate = DateTime.Parse(searchToDate);
                        tours = _unitOfWork.tourKDNDRepository.Find(x => x.Batdau < toDate.AddDays(1)).ToList();

                    }
                    catch (Exception)
                    {
                        return null;
                    }

                }
            }
            // search date
            #endregion

            var list = new List<TourNDDTO>();
            //var companies = _unitOfWork.khachHangRepository.GetAll();
            var dmchinhanhs = _unitOfWork.dmChiNhanhRepository.GetAll();
            var loaiTours = _unitOfWork.tourKindRepository.GetAll();
            var cacNoiDungHuyTours = _unitOfWork.cacNoiDungHuyTourRepository.GetAll();

            if (tours == null)
            {
                return null;
            }
            else
            {
                if (!string.IsNullOrEmpty(chiNhanh)) // chiNhanh
                {
                    tours = tours.Where(x => x.Chinhanh == chiNhanh).ToList();
                    //tours = tours.Where(item1 => chiNhanhs.Any(item2 => item1.Chinhanh == item2)).ToList();

                    if (!string.IsNullOrEmpty(username))
                    {
                        tours = tours.Where(x => x.Nguoitao == username).ToList();
                    }
                }
            }
            foreach (var item in tours)
            {
                var tourDto = new TourNDDTO();

                tourDto.Idtour = item.Idtour;
                tourDto.Sgtcode = item.Sgtcode;
                tourDto.Chudetour = item.Chudetour;
                tourDto.Ngaytao = item.Ngaytao;
                tourDto.Nguoitao = item.Nguoitao;
                tourDto.Batdau = item.Batdau;
                tourDto.Ketthuc = item.Ketthuc;
                tourDto.Tuyentq = item.Tuyentq;
                tourDto.Diemtq = item.Diemtq;
                tourDto.Sokhachdk = item.Sokhachdk;
                tourDto.Sokhachtt = item.Sokhachtt;
                tourDto.Doanhthudk = item.Doanhthudk;
                tourDto.Doanhthutt = item.Doanhthutt;
                tourDto.Makh = item.Makh;
                tourDto.Tenkh = item.Tenkh;
                tourDto.Diachi = item.Diachi;
                tourDto.Dienthoai = item.Dienthoai;
                tourDto.Fax = item.Fax;
                tourDto.Email = item.Email;

                if (item.Ngaydamphan.HasValue)
                {
                    tourDto.Ngaydamphan = item.Ngaydamphan.Value;
                }

                tourDto.Hinhthucgiaodich = item.Hinhthucgiaodich;
                if (item.Ngaykyhopdong.HasValue)
                {
                    tourDto.Ngaykyhopdong = item.Ngaykyhopdong.Value;
                }

                tourDto.Nguoikyhopdong = item.Nguoikyhopdong;
                if (item.Hanxuatvmb.HasValue)
                {
                    tourDto.Hanxuatvmb = item.Hanxuatvmb.Value;
                }
                if (item.Ngaythanhlyhd.HasValue)
                {
                    tourDto.Ngaythanhlyhd = item.Ngaythanhlyhd.Value;
                }
                tourDto.Noidungthanhlyhd = item.Noidungthanhlyhd;
                tourDto.Dichvu = item.Dichvu;
                tourDto.Loaitourid = item.Loaitourid;
                tourDto.Trangthai = item.Trangthai;
                tourDto.Ngaysua = item.Ngaysua;
                tourDto.Nguoisua = item.Nguoisua;
                tourDto.Chinhanh = item.Chinhanh;
                tourDto.ChiNhanhDh = item.ChiNhanhDh;
                if (item.Ngaynhandutien.HasValue)
                {
                    tourDto.Ngaynhandutien = item.Ngaynhandutien.Value;
                }
                tourDto.Lidonhandu = item.Lidonhandu;
                tourDto.Sohopdong = item.Sohopdong;
                tourDto.Laichuave = item.Laichuave;
                tourDto.Laigomve = item.Laigomve;
                tourDto.Laithuctegomve = item.Laithuctegomve;
                tourDto.Nguyennhanhuythau = item.Nguyennhanhuythau;
                tourDto.Nguontour = item.Nguontour;
                tourDto.Filekhachditour = item.Filekhachditour;
                tourDto.Filevemaybay = item.Filevemaybay;
                tourDto.Filebiennhan = item.Filebiennhan;
                tourDto.Nguoidaidien = item.Nguoidaidien;
                tourDto.Doitacnuocngoai = item.Doitacnuocngoai;
                tourDto.Ngayhuytour = item.Ngayhuytour;
                tourDto.LogFile = item.LogFile;

                list.Add(tourDto);
            }

            list = list.Where(x => string.IsNullOrEmpty(x.Nguyennhanhuythau)).OrderBy(x => x.Batdau).ToList();
            var count = list.Count();

            return list;

        }
        
        // DoanhSoTheoThang_OB
        public IEnumerable<TourOBDTO> DoanhSoTheoThang_OB(string searchFromDate, string searchToDate, 
            string chiNhanh, string username)
        {

            var tours = new List<Data.Models_KDOB.Tour>();// _unitOfWork.tourRepository.Find(item1 => listCN.Any(item2 => item1.ChiNhanhTaoId == item2.Id)).ToList();
            #region search date
            // search date
            DateTime fromDate, toDate;
            if (!string.IsNullOrEmpty(searchFromDate) && !string.IsNullOrEmpty(searchToDate))
            {

                try
                {
                    fromDate = DateTime.Parse(searchFromDate);
                    toDate = DateTime.Parse(searchToDate);

                    if (fromDate > toDate)
                    {
                        return null;
                    }
                    tours = _unitOfWork.tourKDOBRepository.Find(x => x.Batdau >= fromDate &&
                                       x.Batdau < toDate.AddDays(1)).ToList();
                }
                catch (Exception)
                {

                    return null;
                }


                //list.Where(x => x.NgayTao >= fromDate && x.NgayTao < (toDate.AddDays(1))/*.ToPagedList(page, pageSize)*/;



            }
            else
            {
                if (!string.IsNullOrEmpty(searchFromDate))
                {
                    try
                    {
                        fromDate = DateTime.Parse(searchFromDate);
                        tours = _unitOfWork.tourKDOBRepository.Find(x => x.Batdau >= fromDate).ToList();
                    }
                    catch (Exception)
                    {
                        return null;
                    }

                }
                if (!string.IsNullOrEmpty(searchToDate))
                {
                    try
                    {
                        toDate = DateTime.Parse(searchToDate);
                        tours = _unitOfWork.tourKDOBRepository.Find(x => x.Batdau < toDate.AddDays(1)).ToList();

                    }
                    catch (Exception)
                    {
                        return null;
                    }

                }
            }
            // search date
            #endregion

            var list = new List<TourOBDTO>();
            //var companies = _unitOfWork.khachHangRepository.GetAll();
            var dmchinhanhs = _unitOfWork.dmChiNhanhRepository.GetAll();
            var loaiTours = _unitOfWork.tourKindRepository.GetAll();
            var cacNoiDungHuyTours = _unitOfWork.cacNoiDungHuyTourRepository.GetAll();

            if (tours == null)
            {
                return null;
            }
            else
            {
                if (!string.IsNullOrEmpty(chiNhanh)) // chiNhanhs: cn QL
                {
                    tours = tours.Where(x => x.Chinhanh == chiNhanh).ToList();
                    //tours = tours.Where(item1 => chiNhanhs.Any(item2 => item1.Chinhanh == item2)).ToList();

                    if (!string.IsNullOrEmpty(username))
                    {
                        tours = tours.Where(x => x.Nguoitao == username).ToList();
                    }
                }
            }
            foreach (var item in tours)
            {
                var tourDto = new TourOBDTO();

                tourDto.Idtour = item.Idtour;
                tourDto.Sgtcode = item.Sgtcode;
                tourDto.Chudetour = item.Chudetour;
                tourDto.Ngaytao = item.Ngaytao;
                tourDto.Nguoitao = item.Nguoitao;
                tourDto.Batdau = item.Batdau;
                tourDto.Ketthuc = item.Ketthuc;
                tourDto.Tuyentq = item.Tuyentq;
                tourDto.Diemtq = item.Diemtq;
                tourDto.Sokhachdk = item.Sokhachdk;
                tourDto.Sokhachtt = item.Sokhachtt;
                tourDto.Doanhthudk = item.Doanhthudk;
                tourDto.Doanhthutt = item.Doanhthutt;
                tourDto.Makh = item.Makh;
                tourDto.Tenkh = item.Tenkh;
                tourDto.Diachi = item.Diachi;
                tourDto.Dienthoai = item.Dienthoai;
                tourDto.Fax = item.Fax;
                tourDto.Email = item.Email;

                if (item.Ngaydamphan.HasValue)
                {
                    tourDto.Ngaydamphan = item.Ngaydamphan.Value;
                }

                tourDto.Hinhthucgiaodich = item.Hinhthucgiaodich;
                if (item.Ngaykyhopdong.HasValue)
                {
                    tourDto.Ngaykyhopdong = item.Ngaykyhopdong.Value;
                }

                tourDto.Nguoikyhopdong = item.Nguoikyhopdong;
                if (item.Hanxuatvmb.HasValue)
                {
                    tourDto.Hanxuatvmb = item.Hanxuatvmb.Value;
                }
                if (item.Ngaythanhlyhd.HasValue)
                {
                    tourDto.Ngaythanhlyhd = item.Ngaythanhlyhd.Value;
                }
                tourDto.Noidungthanhlyhd = item.Noidungthanhlyhd;
                tourDto.Dichvu = item.Dichvu;
                tourDto.Loaitourid = item.Loaitourid;
                tourDto.Trangthai = item.Trangthai;
                tourDto.Ngaysua = item.Ngaysua;
                tourDto.Nguoisua = item.Nguoisua;
                tourDto.Chinhanh = item.Chinhanh;
                tourDto.ChiNhanhDh = item.ChiNhanhDh;
                if (item.Ngaynhandutien.HasValue)
                {
                    tourDto.Ngaynhandutien = item.Ngaynhandutien.Value;
                }
                tourDto.Lidonhandu = item.Lidonhandu;
                tourDto.Sohopdong = item.Sohopdong;
                tourDto.Laichuave = item.Laichuave;
                tourDto.Laigomve = item.Laigomve;
                tourDto.Laithuctegomve = item.Laithuctegomve;
                tourDto.Nguyennhanhuythau = item.Nguyennhanhuythau;
                tourDto.Nguontour = item.Nguontour;
                tourDto.Filekhachditour = item.Filekhachditour;
                tourDto.Filevemaybay = item.Filevemaybay;
                tourDto.Filebiennhan = item.Filebiennhan;
                tourDto.Nguoidaidien = item.Nguoidaidien;
                tourDto.Doitacnuocngoai = item.Doitacnuocngoai;
                tourDto.Ngayhuytour = item.Ngayhuytour;
                //tourDto.LogFile = item.LogFile;

                list.Add(tourDto);
            }

            list = list.Where(x => string.IsNullOrEmpty(x.Nguyennhanhuythau)).OrderBy(x => x.Batdau).ToList();
            var count = list.Count();

            return list;

        }

        /// DoanhSoTheoThiTruong_IB
        public IEnumerable<TourIBDTO> DoanhSoTheoThiTruong_IB(string searchFromDate, string searchToDate, 
            Dmchinhanh dmchinhanh, List<string> thiTruongs, string username)
        {

            var list = new List<TourIBDTO>();
            //var tours = _unitOfWork.tourRepository.GetAll();
            var tours = new List<Data.Models_KDIB.Tours>();
            //var companies = _unitOfWork.khachHangRepository.GetAll();
            var chiNhanhs = _unitOfWork.dmChiNhanhRepository.GetAll();
            var loaiTours = _unitOfWork.tourKindRepository.GetAll();
            var cacNoiDungHuyTours = _unitOfWork.cacNoiDungHuyTourRepository.GetAll();
            var users = _unitOfWork.userIBRepository.GetUsers();

            //// bo da huy tour
            //tours = tours.Where(x => x.HuyTour != true).OrderByDescending(x => x.NgayTao).ToList();

            //if (tours == null)
            //{
            //    return null;
            //}

            #region search date
            // search date
            DateTime fromDate, toDate;
            if (!string.IsNullOrEmpty(searchFromDate) && !string.IsNullOrEmpty(searchToDate))
            {

                try
                {
                    fromDate = DateTime.Parse(searchFromDate);
                    toDate = DateTime.Parse(searchToDate);

                    if (fromDate > toDate)
                    {
                        return null;
                    }
                    tours = _unitOfWork.tourKDIBRepository.Find(x => x.NgayDen >= fromDate &&
                                       x.NgayDen < toDate.AddDays(1)).ToList();
                }
                catch (Exception)
                {

                    return null;
                }


                //list.Where(x => x.NgayTao >= fromDate && x.NgayTao < (toDate.AddDays(1))/*.ToPagedList(page, pageSize)*/;



            }
            else
            {
                if (!string.IsNullOrEmpty(searchFromDate))
                {
                    try
                    {
                        fromDate = DateTime.Parse(searchFromDate);
                        tours = _unitOfWork.tourKDIBRepository.Find(x => x.NgayDen >= fromDate).ToList();
                    }
                    catch (Exception)
                    {
                        return null;
                    }

                }
                if (!string.IsNullOrEmpty(searchToDate))
                {
                    try
                    {
                        toDate = DateTime.Parse(searchToDate);
                        tours = _unitOfWork.tourKDIBRepository.Find(x => x.NgayDen < toDate.AddDays(1)).ToList();

                    }
                    catch (Exception)
                    {
                        return null;
                    }

                }
            }
            // search date
            #endregion

            // loc theo chinhan
            if(dmchinhanh != null)
            {
                tours = tours.Where(x => x.ChiNhanhTaoId == dmchinhanh.Id).ToList();
                //tours = tours.Where(x => dmchinhanhs.Any(y => x.ChiNhanhTaoId == y.Id)).ToList();
                // loc theo thiTruong
                if (thiTruongs.Count > 0)
                {
                    //List<string> thiTruongList = new List<string>();
                    //var usernames = list.Select(x => x.NguoiTao).Distinct();
                    //foreach (var item in usernames)
                    //{
                    //    var thiTruong = _unitOfWork.userRepository.Find(x => x.Username == item).FirstOrDefault().PhongBanId;
                    //    thiTruongList.Add(thiTruong);
                    //}

                    IEnumerable<Data.Models_KDIB.Users> usernames = _unitOfWork.userIBRepository.Find(x => thiTruongs.Any(y => y == x.PhongBanId)); // tat ca user trong thitruong
                    tours = tours.Where(item1 => usernames.Any(item2 => item1.NguoiTao == item2.Username)).ToList();
                }
                if (!string.IsNullOrEmpty(username))
                {
                    tours = tours.Where(x => x.NguoiTao == username).ToList();
                }
            }

            foreach (var item in tours)
            {
                var tourDto = new TourIBDTO();

                tourDto.Id = item.Id;
                tourDto.Sgtcode = item.Sgtcode;
                tourDto.KhachLe = item.KhachLe.Value;
                tourDto.ChuDeTour = item.ChuDeTour;
                tourDto.ThiTruong = item.PhongDh;
                tourDto.NgayKhoa = item.NgayKhoa;
                tourDto.NguoiKhoa = item.NguoiKhoa;
                tourDto.NgayTao = item.NgayTao;
                tourDto.NguoiTao = item.NguoiTao;
                tourDto.NgayDen = item.NgayDen;
                tourDto.NgayDi = item.NgayDi;
                tourDto.TuyenTQ = item.TuyenTq;
                tourDto.SoKhachDK = item.SoKhachDk;
                tourDto.DoanhThuDK = item.DoanhThuDk * (item.TyGia ?? 1);
                tourDto.CompanyId = item.MaKh;// _unitOfWork.companyRepository.Find(x => x.CompanyId == item.MaKh).FirstOrDefault().Name;
                tourDto.TenKh = item.TenKh;

                if (item.NgayDamPhan.HasValue)
                {
                    tourDto.NgayDamPhan = item.NgayDamPhan.Value;
                }

                tourDto.HinhThucGiaoDich = item.HinhThucGiaoDich;
                if (item.NgayKyHopDong.HasValue)
                {
                    tourDto.NgayKyHopDong = item.NgayKyHopDong.Value;
                }

                tourDto.NguoiKyHopDong = item.NguoiKyHopDong;
                if (item.HanXuatVe.HasValue)
                {
                    tourDto.HanXuatVe = item.HanXuatVe.Value;
                }
                if (item.NgayThanhLyHd.HasValue)
                {
                    tourDto.NgayThanhLyHD = item.NgayThanhLyHd.Value;
                }

                tourDto.SoKhachTT = item.SoKhachTt;
                tourDto.SKTreEm = item.SktreEm;
                tourDto.DoanhThuTT = item.DoanhThuTt * (item.TyGia ?? 1);
                tourDto.ChuongTrinhTour = item.ChuongTrinhTour;
                tourDto.NoiDungThanhLyHD = item.NoiDungThanhLyHd;
                tourDto.DichVu = item.DichVu;
                tourDto.DaiLy = item.DaiLy;
                tourDto.TrangThai = item.TrangThai;
                tourDto.NgaySua = item.NgaySua;
                tourDto.NguoiSua = item.NguoiSua;
                tourDto.TenLoaiTour = loaiTours.Where(x => x.Id == item.LoaiTourId).FirstOrDefault().TourkindInf;
                tourDto.MaCNTao = (item.ChiNhanhTaoId == 0) ? "" : chiNhanhs.FirstOrDefault(x => x.Id == item.ChiNhanhTaoId).Macn;// chiNhanhs.Where(x => x.Id == item.ChiNhanhTaoId).FirstOrDefault().Macn;
                if (item.NgayNhanDuTien.HasValue)
                {
                    tourDto.NgayNhanDuTien = item.NgayNhanDuTien.Value;
                }

                tourDto.LyDoNhanDu = item.LyDoNhanDu;
                tourDto.SoHopDong = item.SoHopDong;
                tourDto.LaiChuaVe = item.LaiChuaVe;
                tourDto.LaiGomVe = item.LaiGomVe;
                tourDto.LaiThucTeGomVe = item.LaiThucTeGomVe;
                tourDto.NguonTour = item.NguonTour;
                tourDto.FileKhachDiTour = item.FileKhachDiTour;
                tourDto.FileVeMayBay = item.FileVeMayBay;
                tourDto.FileBienNhan = item.FileBienNhan;
                tourDto.NguoiDaiDien = item.NguoiDaiDien;
                tourDto.DoiTacNuocNgoai = item.DoiTacNuocNgoai;
                tourDto.MaCNDH = chiNhanhs.Where(x => x.Id == item.ChiNhanhDhid).FirstOrDefault().Macn;
                if (item.NgayHuyTour.HasValue)
                {
                    tourDto.NgayHuyTour = item.NgayHuyTour.Value;
                }
                tourDto.HuyTour = item.HuyTour;
                tourDto.NDHuyTour = (item.NdhuyTourId == 0) ? "" : cacNoiDungHuyTours.Where(x => x.Id == item.NdhuyTourId).FirstOrDefault().NoiDung;
                tourDto.GhiChu = item.GhiChu;
                tourDto.LoaiTien = item.LoaiTien;
                tourDto.TyGia = item.TyGia;
                tourDto.LogFile = item.LogFile;

                tourDto.ThiTruongByNguoiTao = users.Where(x => x.Username == item.NguoiTao).FirstOrDefault().PhongBanId;// _unitOfWork.userIBRepository.Find(x => x.Username == item.NguoiTao).FirstOrDefault().PhongBanId;

                list.Add(tourDto);
            }

            //// loc theo thiTruong
            //if (thiTruongs.Count > 0)
            //{
            //    List<string> thiTruongList = new List<string>();
            //    var usernames = list.Select(x => x.NguoiTao).Distinct();
            //    foreach (var item in usernames)
            //    {
            //        var thiTruong = _unitOfWork.userRepository.Find(x => x.Username == item).FirstOrDefault().PhongBanId;
            //        thiTruongList.Add(thiTruong);
            //    }

            //    list = list.Where(item1 => thiTruongList.Any(item2 => item1.ThiTruongByNguoiTao == item2)).ToList();
            //}

            var count = list.Count();

            return list;

        }

    /// DoanhSoTheoSale_IB
    public IEnumerable<TourIBDTO> DoanhSoTheoSale_IB(string searchFromDate, string searchToDate, 
            Dmchinhanh dmchinhanh, List<string> thiTruongs, string username)
        {

            var list = new List<TourIBDTO>();
            //var tours = _unitOfWork.tourRepository.GetAll();
            var tours = new List<Data.Models_KDIB.Tours>();
            //var companies = _unitOfWork.khachHangRepository.GetAll();
            var chiNhanhs = _unitOfWork.dmChiNhanhRepository.GetAll();
            var loaiTours = _unitOfWork.tourKindRepository.GetAll();
            var cacNoiDungHuyTours = _unitOfWork.cacNoiDungHuyTourRepository.GetAll();
            var users = _unitOfWork.userIBRepository.GetUsers();

            //// bo da huy tour
            //tours = tours.Where(x => x.HuyTour != true).OrderByDescending(x => x.NgayTao).ToList();

            //if (tours == null)
            //{
            //    return null;
            //}

            #region search date
            // search date
            DateTime fromDate, toDate;
            if (!string.IsNullOrEmpty(searchFromDate) && !string.IsNullOrEmpty(searchToDate))
            {

                try
                {
                    fromDate = DateTime.Parse(searchFromDate);
                    toDate = DateTime.Parse(searchToDate);

                    if (fromDate > toDate)
                    {
                        return null;
                    }
                    tours = _unitOfWork.tourKDIBRepository.Find(x => x.NgayDen >= fromDate &&
                                       x.NgayDen < toDate.AddDays(1)).ToList();
                }
                catch (Exception)
                {

                    return null;
                }


                //list.Where(x => x.NgayTao >= fromDate && x.NgayTao < (toDate.AddDays(1))/*.ToPagedList(page, pageSize)*/;



            }
            else
            {
                if (!string.IsNullOrEmpty(searchFromDate))
                {
                    try
                    {
                        fromDate = DateTime.Parse(searchFromDate);
                        tours = _unitOfWork.tourKDIBRepository.Find(x => x.NgayDen >= fromDate).ToList();
                    }
                    catch (Exception)
                    {
                        return null;
                    }

                }
                if (!string.IsNullOrEmpty(searchToDate))
                {
                    try
                    {
                        toDate = DateTime.Parse(searchToDate);
                        tours = _unitOfWork.tourKDIBRepository.Find(x => x.NgayDen < toDate.AddDays(1)).ToList();

                    }
                    catch (Exception)
                    {
                        return null;
                    }

                }
            }
            // search date
            #endregion

            if(tours.Count == 0)
            {
                return null;
            }
            else
            {
                // loc theo chinhanh
                if (dmchinhanh != null)
                {
                    tours = tours.Where(x => x.ChiNhanhTaoId == dmchinhanh.Id).ToList();

                    // loc theo thiTruong
                    if (thiTruongs.Count > 0)
                    {
                        
                        IEnumerable<Data.Models_KDIB.Users> usernames = _unitOfWork.userIBRepository.Find(x => thiTruongs.Any(y => y == x.PhongBanId)); // tat ca user trong thitruong
                        tours = tours.Where(item1 => usernames.Any(item2 => item1.NguoiTao == item2.Username)).ToList();
                    }
                    if (!string.IsNullOrEmpty(username))
                    {
                        tours = tours.Where(x => x.NguoiTao == username).ToList();
                    }
                }
                
            }
            
            foreach (var item in tours)
            {
                var tourDto = new TourIBDTO();

                tourDto.Id = item.Id;
                tourDto.Sgtcode = item.Sgtcode;
                tourDto.KhachLe = item.KhachLe.Value;
                tourDto.ChuDeTour = item.ChuDeTour;
                tourDto.ThiTruong = item.PhongDh;
                tourDto.NgayKhoa = item.NgayKhoa;
                tourDto.NguoiKhoa = item.NguoiKhoa;
                tourDto.NgayTao = item.NgayTao;
                tourDto.NguoiTao = item.NguoiTao;
                tourDto.NgayDen = item.NgayDen;
                tourDto.NgayDi = item.NgayDi;
                tourDto.TuyenTQ = item.TuyenTq;
                tourDto.SoKhachDK = item.SoKhachDk;
                tourDto.DoanhThuDK = item.DoanhThuDk * (item.TyGia ?? 1);
                tourDto.CompanyId = item.MaKh;// _unitOfWork.companyRepository.Find(x => x.CompanyId == item.MaKh).FirstOrDefault().Name;
                tourDto.TenKh = item.TenKh;// _unitOfWork.companyRepository.Find(x => x.CompanyId == item.MaKh).FirstOrDefault().Name;

                if (item.NgayDamPhan.HasValue)
                {
                    tourDto.NgayDamPhan = item.NgayDamPhan.Value;
                }

                tourDto.HinhThucGiaoDich = item.HinhThucGiaoDich;
                if (item.NgayKyHopDong.HasValue)
                {
                    tourDto.NgayKyHopDong = item.NgayKyHopDong.Value;
                }

                tourDto.NguoiKyHopDong = item.NguoiKyHopDong;
                if (item.HanXuatVe.HasValue)
                {
                    tourDto.HanXuatVe = item.HanXuatVe.Value;
                }
                if (item.NgayThanhLyHd.HasValue)
                {
                    tourDto.NgayThanhLyHD = item.NgayThanhLyHd.Value;
                }

                tourDto.SoKhachTT = item.SoKhachTt;
                tourDto.SKTreEm = item.SktreEm;
                tourDto.DoanhThuTT = item.DoanhThuTt * (item.TyGia ?? 1);
                tourDto.ChuongTrinhTour = item.ChuongTrinhTour;
                tourDto.NoiDungThanhLyHD = item.NoiDungThanhLyHd;
                tourDto.DichVu = item.DichVu;
                tourDto.DaiLy = item.DaiLy;
                tourDto.TrangThai = item.TrangThai;
                tourDto.NgaySua = item.NgaySua;
                tourDto.NguoiSua = item.NguoiSua;
                tourDto.TenLoaiTour = loaiTours.Where(x => x.Id == item.LoaiTourId).FirstOrDefault().TourkindInf;
                tourDto.MaCNTao = (item.ChiNhanhTaoId == 0) ? "" : _unitOfWork.dmChiNhanhRepository.Find(x => x.Id == item.ChiNhanhTaoId).FirstOrDefault().Macn;// chiNhanhs.Where(x => x.Id == item.ChiNhanhTaoId).FirstOrDefault().Macn;
                if (item.NgayNhanDuTien.HasValue)
                {
                    tourDto.NgayNhanDuTien = item.NgayNhanDuTien.Value;
                }

                tourDto.LyDoNhanDu = item.LyDoNhanDu;
                tourDto.SoHopDong = item.SoHopDong;
                tourDto.LaiChuaVe = item.LaiChuaVe;
                tourDto.LaiGomVe = item.LaiGomVe;
                tourDto.LaiThucTeGomVe = item.LaiThucTeGomVe;
                tourDto.NguonTour = item.NguonTour;
                tourDto.FileKhachDiTour = item.FileKhachDiTour;
                tourDto.FileVeMayBay = item.FileVeMayBay;
                tourDto.FileBienNhan = item.FileBienNhan;
                tourDto.NguoiDaiDien = item.NguoiDaiDien;
                tourDto.DoiTacNuocNgoai = item.DoiTacNuocNgoai;
                tourDto.MaCNDH = chiNhanhs.Where(x => x.Id == item.ChiNhanhDhid).FirstOrDefault().Macn;
                if (item.NgayHuyTour.HasValue)
                {
                    tourDto.NgayHuyTour = item.NgayHuyTour.Value;
                }
                tourDto.HuyTour = item.HuyTour;
                tourDto.NDHuyTour = (item.NdhuyTourId == 0) ? "" : cacNoiDungHuyTours.Where(x => x.Id == item.NdhuyTourId).FirstOrDefault().NoiDung;
                tourDto.GhiChu = item.GhiChu;
                tourDto.LoaiTien = item.LoaiTien;
                tourDto.TyGia = item.TyGia;
                tourDto.LogFile = item.LogFile;

                tourDto.ThiTruongByNguoiTao = users.Where(x => x.Username == item.NguoiTao).FirstOrDefault().PhongBanId;// _unitOfWork.userIBRepository.Find(x => x.Username == item.NguoiTao).FirstOrDefault().PhongBanId;

                list.Add(tourDto);
            }

            var count = list.Count();

            return list;

        }

        // DoanhSoTheoNgay_IB
        public IEnumerable<TourIBDTO> DoanhSoTheoNgay_IB(string searchFromDate, string searchToDate, string loaiTour,
            Dmchinhanh dmchinhanh, List<string> phongBanQLs, string username)
        {
            var tours = new List<Tours>();
            #region search date
            // search date
            DateTime fromDate, toDate;
            if (!string.IsNullOrEmpty(searchFromDate) && !string.IsNullOrEmpty(searchToDate))
            {

                try
                {
                    fromDate = DateTime.Parse(searchFromDate);
                    toDate = DateTime.Parse(searchToDate);

                    if (fromDate > toDate)
                    {
                        return null;
                    }
                    //list = list.Where(x => x.NgayTao >= fromDate &&
                    //                   x.NgayTao < toDate.AddDays(1)).ToList();
                    tours = _unitOfWork.tourKDIBRepository.Find(x => x.NgayDen >= fromDate &&
                                       x.NgayDen < toDate.AddDays(1)).ToList(); // tungay denngay
                }
                catch (Exception)
                {

                    return null;
                }

            }
            else
            {
                if (!string.IsNullOrEmpty(searchFromDate))
                {
                    try
                    {
                        fromDate = DateTime.Parse(searchFromDate);
                        //list = list.Where(x => x.NgayTao >= fromDate).ToList();
                        tours = _unitOfWork.tourKDIBRepository.Find(x => x.NgayDen >= fromDate).ToList();
                    }
                    catch (Exception)
                    {
                        return null;
                    }

                }
                if (!string.IsNullOrEmpty(searchToDate))
                {
                    try
                    {
                        toDate = DateTime.Parse(searchToDate);
                        tours = _unitOfWork.tourKDIBRepository.Find(x => x.NgayDen < toDate.AddDays(1)).ToList();

                    }
                    catch (Exception)
                    {
                        return null;
                    }

                }
            }
            // search date
            #endregion

            var list = new List<TourIBDTO>();
            //var tours = _unitOfWork.tourRepository.GetAll();
            //var companies = _unitOfWork.khachHangRepository.GetAll();
            var dmchinhanhs = _unitOfWork.dmChiNhanhRepository.GetAll();
            var loaiTours = _unitOfWork.tourKindRepository.GetAll();
            var cacNoiDungHuyTours = _unitOfWork.cacNoiDungHuyTourRepository.GetAll();

            // bo da huy tour
            tours = tours.Where(x => x.HuyTour != true).OrderByDescending(x => x.NgayTao).ToList();

            if (tours == null)
            {
                return null;
            }
            else
            {
                if (dmchinhanh != null)
                {
                    tours = tours.Where(x => x.ChiNhanhTaoId == dmchinhanh.Id).ToList();
                    //tours = tours.Where(item1 => listCN.Any(item2 => item1.ChiNhanhTaoId == item2.Id)).ToList();
                    if (phongBanQLs.Count > 0) // chi lay phong ban QL
                    {
                        var usernames = _unitOfWork.userRepository.Find(x => phongBanQLs.Any(y => y == x.PhongBanId)); // tat ca user trong thitruong
                        tours = tours.Where(x => usernames.Any(y => x.NguoiTao == y.Username)).ToList();

                    }
                    if (!string.IsNullOrEmpty(username)) // ko QL ai het ==> user thuong
                    {
                        tours = tours.Where(x => x.NguoiTao == username).ToList();
                    }
                }
            }
            foreach (var item in tours)
            {
                var tourDto = new TourIBDTO();

                tourDto.Id = item.Id;
                tourDto.Sgtcode = item.Sgtcode;
                tourDto.KhachLe = item.KhachLe.Value;
                tourDto.ChuDeTour = item.ChuDeTour;
                tourDto.ThiTruong = item.PhongDh;
                tourDto.NgayKhoa = item.NgayKhoa;
                tourDto.NguoiKhoa = item.NguoiKhoa;
                tourDto.NgayTao = item.NgayTao;
                tourDto.NguoiTao = item.NguoiTao;
                tourDto.NgayDen = item.NgayDen;
                tourDto.NgayDi = item.NgayDi;
                tourDto.TuyenTQ = item.TuyenTq;
                tourDto.SoKhachDK = item.SoKhachDk;
                tourDto.DoanhThuDK = item.DoanhThuDk * (item.TyGia ?? 1);
                tourDto.CompanyId = item.MaKh;// _unitOfWork.companyRepository.Find(x => x.CompanyId == item.MaKh).FirstOrDefault().Name;
                tourDto.TenKh = item.TenKh;// _unitOfWork.companyRepository.Find(x => x.CompanyId == item.MaKh).FirstOrDefault().Name;

                if (item.NgayDamPhan.HasValue)
                {
                    tourDto.NgayDamPhan = item.NgayDamPhan.Value;
                }

                tourDto.HinhThucGiaoDich = item.HinhThucGiaoDich;
                if (item.NgayKyHopDong.HasValue)
                {
                    tourDto.NgayKyHopDong = item.NgayKyHopDong.Value;
                }

                tourDto.NguoiKyHopDong = item.NguoiKyHopDong;
                if (item.HanXuatVe.HasValue)
                {
                    tourDto.HanXuatVe = item.HanXuatVe.Value;
                }
                if (item.NgayThanhLyHd.HasValue)
                {
                    tourDto.NgayThanhLyHD = item.NgayThanhLyHd.Value;
                }

                tourDto.SoKhachTT = item.SoKhachTt;
                tourDto.SKTreEm = item.SktreEm;
                tourDto.DoanhThuTT = item.DoanhThuTt * (item.TyGia ?? 1);
                tourDto.ChuongTrinhTour = item.ChuongTrinhTour;
                tourDto.NoiDungThanhLyHD = item.NoiDungThanhLyHd;
                tourDto.DichVu = item.DichVu;
                tourDto.DaiLy = item.DaiLy;
                tourDto.TrangThai = item.TrangThai;
                tourDto.NgaySua = item.NgaySua;
                tourDto.NguoiSua = item.NguoiSua;
                tourDto.TenLoaiTour = loaiTours.Where(x => x.Id == item.LoaiTourId).FirstOrDefault().TourkindInf;
                tourDto.MaCNTao = (item.ChiNhanhTaoId == 0) ? "" : dmchinhanhs.FirstOrDefault(x => x.Id == item.ChiNhanhTaoId).Macn;
                if (item.NgayNhanDuTien.HasValue)
                {
                    tourDto.NgayNhanDuTien = item.NgayNhanDuTien.Value;
                }

                tourDto.LyDoNhanDu = item.LyDoNhanDu;
                tourDto.SoHopDong = item.SoHopDong;
                tourDto.LaiChuaVe = item.LaiChuaVe;
                tourDto.LaiGomVe = item.LaiGomVe;
                tourDto.LaiThucTeGomVe = item.LaiThucTeGomVe;
                tourDto.NguonTour = item.NguonTour;
                tourDto.FileKhachDiTour = item.FileKhachDiTour;
                tourDto.FileVeMayBay = item.FileVeMayBay;
                tourDto.FileBienNhan = item.FileBienNhan;
                tourDto.NguoiDaiDien = item.NguoiDaiDien;
                tourDto.DoiTacNuocNgoai = item.DoiTacNuocNgoai;
                tourDto.MaCNDH = dmchinhanhs.Where(x => x.Id == item.ChiNhanhDhid).FirstOrDefault().Macn;
                if (item.NgayHuyTour.HasValue)
                {
                    tourDto.NgayHuyTour = item.NgayHuyTour.Value;
                }
                tourDto.HuyTour = item.HuyTour;
                tourDto.NDHuyTour = (item.NdhuyTourId == 0) ? "" : cacNoiDungHuyTours.Where(x => x.Id == item.NdhuyTourId).FirstOrDefault().NoiDung;
                tourDto.GhiChu = item.GhiChu;
                tourDto.LoaiTien = item.LoaiTien;
                tourDto.TyGia = item.TyGia;
                tourDto.LogFile = item.LogFile;

                //tourDto.Invoices = _context.Invoices.Where(x => x.TourId == item.Id).Count();

                list.Add(tourDto);
            }

            // loc theo loai tour
            if (!string.IsNullOrEmpty(loaiTour))
            {
                list = list.Where(x => x.TenLoaiTour.Trim().ToLower() == loaiTour.Trim().ToLower()).ToList();
            }

            // loc theo loai tour

            //// loc theo chinhanh
            //if (maCNs.Count > 0)
            //{
            //    //list = list.Where(x => x.MaCNTao == macn).ToList();
            //    list = list.Where(item1 => maCNs.Any(item2 => item1.MaCNTao == item2)).ToList();
            //}

            var count = list.Count();

            return list;

        }
        
        // DoanhSoTheoNgay_ND
        public IEnumerable<TourNDDTO> DoanhSoTheoNgay_ND(string searchFromDate, string searchToDate, string loaiTour,
            string maCn, string username)
        {
            var tours = new List<ThongKe.Data.Models_KDND.Tour>();
            #region search date
            // search date
            DateTime fromDate, toDate;
            if (!string.IsNullOrEmpty(searchFromDate) && !string.IsNullOrEmpty(searchToDate))
            {

                try
                {
                    fromDate = DateTime.Parse(searchFromDate);
                    toDate = DateTime.Parse(searchToDate);

                    if (fromDate > toDate)
                    {
                        return null;
                    }
                    //list = list.Where(x => x.NgayTao >= fromDate &&
                    //                   x.NgayTao < toDate.AddDays(1)).ToList();
                    tours = _unitOfWork.tourKDNDRepository.Find(x => x.Batdau >= fromDate &&
                                       x.Batdau < toDate.AddDays(1)).ToList(); // tungay denngay
                }
                catch (Exception)
                {

                    return null;
                }


                //list.Where(x => x.NgayTao >= fromDate && x.NgayTao < (toDate.AddDays(1))/*.ToPagedList(page, pageSize)*/;



            }
            else
            {
                if (!string.IsNullOrEmpty(searchFromDate))
                {
                    try
                    {
                        fromDate = DateTime.Parse(searchFromDate);
                        //list = list.Where(x => x.NgayTao >= fromDate).ToList();
                        tours = _unitOfWork.tourKDNDRepository.Find(x => x.Batdau >= fromDate).ToList();
                    }
                    catch (Exception)
                    {
                        return null;
                    }

                }
                if (!string.IsNullOrEmpty(searchToDate))
                {
                    try
                    {
                        toDate = DateTime.Parse(searchToDate);
                        //list = list.Where(x => x.NgayTao < toDate.AddDays(1)).ToList();
                        tours = _unitOfWork.tourKDNDRepository.Find(x => x.Batdau < toDate.AddDays(1)).ToList();

                    }
                    catch (Exception)
                    {
                        return null;
                    }

                }
            }
            // search date
            #endregion

            var list = new List<TourNDDTO>();
            //var tours = _unitOfWork.tourRepository.GetAll();
            //var companies = _unitOfWork.khachHangRepository.GetAll();
            var dmchinhanhs = _unitOfWork.dmChiNhanhRepository.GetAll();
            var loaiTours = _unitOfWork.tourKindRepository.GetAll();
            var cacNoiDungHuyTours = _unitOfWork.cacNoiDungHuyTourRepository.GetAll();

            // bo da huy tour
            tours = tours.Where(x => string.IsNullOrEmpty(x.Nguyennhanhuythau)).OrderBy(x => x.Batdau).ToList();

            if (tours == null)
            {
                return null;
            }
            else
            {
                if (!string.IsNullOrEmpty(maCn))
                {
                    tours = tours.Where(x => x.Chinhanh == maCn).ToList();
                    //tours = tours.Where(item1 => listCN.Any(item2 => item1.Chinhanh == item2)).ToList();
                    if (!string.IsNullOrEmpty(username)) // ko QL ai het ==> user thuong
                    {
                        tours = tours.Where(x => x.Nguoitao == username).ToList();
                    }
                }
            }
            foreach (var item in tours)
            {
                var tourDto = new TourNDDTO();

                tourDto.Idtour = item.Idtour;
                tourDto.Sgtcode = item.Sgtcode;
                tourDto.Chudetour = item.Chudetour;
                tourDto.Ngaytao = item.Ngaytao;
                tourDto.Nguoitao = item.Nguoitao;
                tourDto.Batdau = item.Batdau;
                tourDto.Ketthuc = item.Ketthuc;
                tourDto.Tuyentq = item.Tuyentq;
                tourDto.Diemtq = item.Diemtq;
                tourDto.Sokhachdk = item.Sokhachdk;
                tourDto.Sokhachtt = item.Sokhachtt ?? 0;
                tourDto.Doanhthudk = item.Doanhthudk;
                tourDto.Doanhthutt = item.Doanhthutt ?? 0;
                tourDto.Makh = item.Makh;
                tourDto.Tenkh = item.Tenkh;
                tourDto.Diachi = item.Diachi;
                tourDto.Dienthoai = item.Dienthoai;
                tourDto.Fax = item.Fax;
                tourDto.Email = item.Email;

                if (item.Ngaydamphan.HasValue)
                {
                    tourDto.Ngaydamphan = item.Ngaydamphan.Value;
                }

                tourDto.Hinhthucgiaodich = item.Hinhthucgiaodich;
                if (item.Ngaykyhopdong.HasValue)
                {
                    tourDto.Ngaykyhopdong = item.Ngaykyhopdong.Value;
                }

                tourDto.Nguoikyhopdong = item.Nguoikyhopdong;
                if (item.Hanxuatvmb.HasValue)
                {
                    tourDto.Hanxuatvmb = item.Hanxuatvmb.Value;
                }
                if (item.Ngaythanhlyhd.HasValue)
                {
                    tourDto.Ngaythanhlyhd = item.Ngaythanhlyhd.Value;
                }
                tourDto.Noidungthanhlyhd = item.Noidungthanhlyhd;
                tourDto.Dichvu = item.Dichvu;
                tourDto.Loaitourid = item.Loaitourid;
                tourDto.Trangthai = item.Trangthai;
                tourDto.Ngaysua = item.Ngaysua;
                tourDto.Nguoisua = item.Nguoisua;
                tourDto.Chinhanh = item.Chinhanh;
                tourDto.ChiNhanhDh = item.ChiNhanhDh;
                if (item.Ngaynhandutien.HasValue)
                {
                    tourDto.Ngaynhandutien = item.Ngaynhandutien.Value;
                }
                tourDto.Lidonhandu = item.Lidonhandu;
                tourDto.Sohopdong = item.Sohopdong;
                tourDto.Laichuave = item.Laichuave;
                tourDto.Laigomve = item.Laigomve;
                tourDto.Laithuctegomve = item.Laithuctegomve;
                tourDto.Nguyennhanhuythau = item.Nguyennhanhuythau;
                tourDto.Nguontour = item.Nguontour;
                tourDto.Filekhachditour = item.Filekhachditour;
                tourDto.Filevemaybay = item.Filevemaybay;
                tourDto.Filebiennhan = item.Filebiennhan;
                tourDto.Nguoidaidien = item.Nguoidaidien;
                tourDto.Doitacnuocngoai = item.Doitacnuocngoai;
                tourDto.Ngayhuytour = item.Ngayhuytour;
                tourDto.LogFile = item.LogFile;

                list.Add(tourDto);
            }

            // loc theo loai tour
            if (!string.IsNullOrEmpty(loaiTour)) // loaiTour: ten loaitour
            {
                // Loaitourid == tenloaitour trong bang loaitour của saledoanND
                list = list.Where(x => x.Loaitourid.Trim().ToLower() == loaiTour.Trim().ToLower()).ToList();
            }

            // loc theo loai tour

            //// loc theo chinhanh
            //if (maCNs.Count > 0)
            //{
            //    //list = list.Where(x => x.MaCNTao == macn).ToList();
            //    list = list.Where(item1 => maCNs.Any(item2 => item1.MaCNTao == item2)).ToList();
            //}

            var count = list.Count();

            return list;

        }
        
        // DoanhSoTheoNgay_OB
        public IEnumerable<TourOBDTO> DoanhSoTheoNgay_OB(string searchFromDate, string searchToDate, string loaiTour,
            string maCn, string username)
        {
            var tours = new List<ThongKe.Data.Models_KDOB.Tour>();
            #region search date
            // search date
            DateTime fromDate, toDate;
            if (!string.IsNullOrEmpty(searchFromDate) && !string.IsNullOrEmpty(searchToDate))
            {

                try
                {
                    fromDate = DateTime.Parse(searchFromDate);
                    toDate = DateTime.Parse(searchToDate);

                    if (fromDate > toDate)
                    {
                        return null;
                    }
                    //list = list.Where(x => x.NgayTao >= fromDate &&
                    //                   x.NgayTao < toDate.AddDays(1)).ToList();
                    tours = _unitOfWork.tourKDOBRepository.Find(x => x.Batdau >= fromDate &&
                                       x.Batdau < toDate.AddDays(1)).ToList(); // tungay denngay
                }
                catch (Exception)
                {

                    return null;
                }


                //list.Where(x => x.NgayTao >= fromDate && x.NgayTao < (toDate.AddDays(1))/*.ToPagedList(page, pageSize)*/;



            }
            else
            {
                if (!string.IsNullOrEmpty(searchFromDate))
                {
                    try
                    {
                        fromDate = DateTime.Parse(searchFromDate);
                        //list = list.Where(x => x.NgayTao >= fromDate).ToList();
                        tours = _unitOfWork.tourKDOBRepository.Find(x => x.Batdau >= fromDate).ToList();
                    }
                    catch (Exception)
                    {
                        return null;
                    }

                }
                if (!string.IsNullOrEmpty(searchToDate))
                {
                    try
                    {
                        toDate = DateTime.Parse(searchToDate);
                        //list = list.Where(x => x.NgayTao < toDate.AddDays(1)).ToList();
                        tours = _unitOfWork.tourKDOBRepository.Find(x => x.Batdau < toDate.AddDays(1)).ToList();

                    }
                    catch (Exception)
                    {
                        return null;
                    }

                }
            }
            // search date
            #endregion

            var list = new List<TourOBDTO>();
            //var tours = _unitOfWork.tourRepository.GetAll();
            //var companies = _unitOfWork.khachHangRepository.GetAll();
            var dmchinhanhs = _unitOfWork.dmChiNhanhRepository.GetAll();
            var loaiTours = _unitOfWork.tourKindRepository.GetAll();
            var cacNoiDungHuyTours = _unitOfWork.cacNoiDungHuyTourRepository.GetAll();

            // bo da huy tour
            tours = tours.Where(x => string.IsNullOrEmpty(x.Nguyennhanhuythau)).OrderBy(x => x.Batdau).ToList();

            if (tours == null)
            {
                return null;
            }
            else
            {
                if (!string.IsNullOrEmpty(maCn)) // listCN: cn QL && loc theo chinhanh
                {
                    tours = tours.Where(x => x.Chinhanh == maCn).ToList();
                    //tours = tours.Where(item1 => listCN.Any(item2 => item1.Chinhanh == item2)).ToList();
                    if (!string.IsNullOrEmpty(username)) // ko QL ai het ==> user thuong
                    {
                        tours = tours.Where(x => x.Nguoitao == username).ToList();
                    }
                }
            }
            foreach (var item in tours)
            {
                var tourDto = new TourOBDTO();

                tourDto.Idtour = item.Idtour;
                tourDto.Sgtcode = item.Sgtcode;
                tourDto.Chudetour = item.Chudetour;
                tourDto.Ngaytao = item.Ngaytao;
                tourDto.Nguoitao = item.Nguoitao;
                tourDto.Batdau = item.Batdau;
                tourDto.Ketthuc = item.Ketthuc;
                tourDto.Tuyentq = item.Tuyentq;
                tourDto.Diemtq = item.Diemtq;
                tourDto.Sokhachdk = item.Sokhachdk;
                tourDto.Sokhachtt = item.Sokhachtt ?? 0;
                tourDto.Doanhthudk = item.Doanhthudk;
                tourDto.Doanhthutt = item.Doanhthutt ?? 0;
                tourDto.Makh = item.Makh;
                tourDto.Tenkh = item.Tenkh;
                tourDto.Diachi = item.Diachi;
                tourDto.Dienthoai = item.Dienthoai;
                tourDto.Fax = item.Fax;
                tourDto.Email = item.Email;

                if (item.Ngaydamphan.HasValue)
                {
                    tourDto.Ngaydamphan = item.Ngaydamphan.Value;
                }

                tourDto.Hinhthucgiaodich = item.Hinhthucgiaodich;
                if (item.Ngaykyhopdong.HasValue)
                {
                    tourDto.Ngaykyhopdong = item.Ngaykyhopdong.Value;
                }

                tourDto.Nguoikyhopdong = item.Nguoikyhopdong;
                if (item.Hanxuatvmb.HasValue)
                {
                    tourDto.Hanxuatvmb = item.Hanxuatvmb.Value;
                }
                if (item.Ngaythanhlyhd.HasValue)
                {
                    tourDto.Ngaythanhlyhd = item.Ngaythanhlyhd.Value;
                }
                tourDto.Noidungthanhlyhd = item.Noidungthanhlyhd;
                tourDto.Dichvu = item.Dichvu;
                tourDto.Loaitourid = item.Loaitourid;
                tourDto.Trangthai = item.Trangthai;
                tourDto.Ngaysua = item.Ngaysua;
                tourDto.Nguoisua = item.Nguoisua;
                tourDto.Chinhanh = item.Chinhanh;
                tourDto.ChiNhanhDh = item.ChiNhanhDh;
                if (item.Ngaynhandutien.HasValue)
                {
                    tourDto.Ngaynhandutien = item.Ngaynhandutien.Value;
                }
                tourDto.Lidonhandu = item.Lidonhandu;
                tourDto.Sohopdong = item.Sohopdong;
                tourDto.Laichuave = item.Laichuave;
                tourDto.Laigomve = item.Laigomve;
                tourDto.Laithuctegomve = item.Laithuctegomve;
                tourDto.Nguyennhanhuythau = item.Nguyennhanhuythau;
                tourDto.Nguontour = item.Nguontour;
                tourDto.Filekhachditour = item.Filekhachditour;
                tourDto.Filevemaybay = item.Filevemaybay;
                tourDto.Filebiennhan = item.Filebiennhan;
                tourDto.Nguoidaidien = item.Nguoidaidien;
                tourDto.Doitacnuocngoai = item.Doitacnuocngoai;
                tourDto.Ngayhuytour = item.Ngayhuytour;
                //tourDto.LogFile = item.LogFile;

                list.Add(tourDto);
            }

            // loc theo loai tour
            if (!string.IsNullOrEmpty(loaiTour)) // loaiTour: ten loaitour
            {
                // Loaitourid == tenloaitour trong bang loaitour của saledoanND
                list = list.Where(x => x.Loaitourid.Trim().ToLower() == loaiTour.Trim().ToLower()).ToList();
            }

            // loc theo loai tour

            //// loc theo chinhanh
            //if (maCNs.Count > 0)
            //{
            //    //list = list.Where(x => x.MaCNTao == macn).ToList();
            //    list = list.Where(item1 => maCNs.Any(item2 => item1.MaCNTao == item2)).ToList();
            //}

            var count = list.Count();

            return list;

        }

        public IEnumerable<Company> GetCompanies()
        {
            return _unitOfWork.companyRepository.GetAll();
        }
        
        public IEnumerable<Dmchinhanh> GetAllChiNhanh()
        {
            return _unitOfWork.dmChiNhanhRepository.GetAll();
        }

        public async Task<Role> GetRoleById(int id)
        {
            return await _unitOfWork.roleRepository.GetRoleById(id);
        }

        public async Task<IEnumerable<Role>> GetRoles()
        {
            return await _unitOfWork.roleRepository.GetRoles();
        }

        public IEnumerable<Tourkind> GetTourinds()
        {
            return _unitOfWork.tourKindRepository.GetAll();
        }

        public IEnumerable<Loaitour> GetLoaiTours()
        {
            return _unitOfWork.tourKDNDRepository.GetLoaitours();
        }

        public IEnumerable<DoanhthuSaleTuyen> ListSaleTheoTuyenThamQuan(string tungay, string denngay, string chiNhanh, string tuyentq, string khoi)
        {
            return _unitOfWork.thongKeRepository.ListSaleTheoTuyenThamQuan(tungay, denngay, tuyentq, khoi)
                .Where(x => x.Chinhanh == chiNhanh);
        }

    }
}
