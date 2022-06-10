using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models_KDIB
{
    public partial class Tours
    {
        public long Id { get; set; }
        public string Sgtcode { get; set; }
        public string ChuDeTour { get; set; }
        public string PhongDh { get; set; }
        public DateTime NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public string TuyenTq { get; set; }
        public int SoKhachDk { get; set; }
        public DateTime? NgayDamPhan { get; set; }
        public string HinhThucGiaoDich { get; set; }
        public DateTime? NgayKyHopDong { get; set; }
        public string NguoiKyHopDong { get; set; }
        public DateTime? HanXuatVe { get; set; }
        public DateTime? NgayThanhLyHd { get; set; }
        public int SoKhachTt { get; set; }
        public decimal DoanhThuTt { get; set; }
        public string ChuongTrinhTour { get; set; }
        public string NoiDungThanhLyHd { get; set; }
        public string DichVu { get; set; }
        public string DaiLy { get; set; }
        public string TrangThai { get; set; }
        public DateTime? NgaySua { get; set; }
        public string NguoiSua { get; set; }
        public DateTime? NgayNhanDuTien { get; set; }
        public string LyDoNhanDu { get; set; }
        public string SoHopDong { get; set; }
        public decimal LaiChuaVe { get; set; }
        public decimal LaiGomVe { get; set; }
        public decimal LaiThucTeGomVe { get; set; }
        public string NguonTour { get; set; }
        public string FileKhachDiTour { get; set; }
        public string FileVeMayBay { get; set; }
        public string FileBienNhan { get; set; }
        public string NguoiDaiDien { get; set; }
        public string DoiTacNuocNgoai { get; set; }
        public string LogFile { get; set; }
        public string GhiChu { get; set; }
        public bool? KhachLe { get; set; }
        public string LoaiTien { get; set; }
        public int LoaiTourId { get; set; }
        public long? NdhuyTourId { get; set; }
        public DateTime NgayDen { get; set; }
        public DateTime NgayDi { get; set; }
        public DateTime? NgayKhoa { get; set; }
        public string NguoiKhoa { get; set; }
        public int SktreEm { get; set; }
        public decimal? TyGia { get; set; }
        public int ChiNhanhDhid { get; set; }
        public int ChiNhanhTaoId { get; set; }
        public string DiaChi { get; set; }
        public string DienThoai { get; set; }
        public decimal DoanhThuDk { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string MaKh { get; set; }
        public DateTime? NgayHuyTour { get; set; }
        public string TenKh { get; set; }
        public string PhongBanMaCode { get; set; }
        public string LoaiKhach { get; set; }
        public bool? HuyTour { get; set; }
    }
}
