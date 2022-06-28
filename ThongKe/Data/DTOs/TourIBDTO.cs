using System;
using System.ComponentModel;

namespace ThongKe.Data.DTOs
{
    public class TourIBDTO
    {
        public long Id { get; set; }

        public string Sgtcode { get; set; }

        [DisplayName("Khách lẻ")]
        public bool KhachLe { get; set; }

        [DisplayName("Chủ đề tour")]
        public string ChuDeTour { get; set; }

        // PhongBan -> qltour
        [DisplayName("Thị Trường")]
        public string ThiTruong { get; set; }

        public DateTime? NgayKhoa { get; set; }

        [DisplayName("Người khóa")]
        public string NguoiKhoa { get; set; }

        public DateTime NgayTao { get; set; }

        [DisplayName("Người tạo")]
        public string NguoiTao { get; set; }

        [DisplayName("Bắt đầu")]
        public DateTime NgayDen { get; set; }

        [DisplayName("Kết thúc")]
        public DateTime NgayDi { get; set; }

        [DisplayName("Tuyến TQ")]
        public string TuyenTQ { get; set; }

        [DisplayName("Số khách DK")]
        public int SoKhachDK { get; set; }

        [DisplayName("Doanh thu DK")]
        public decimal DoanhThuDK { get; set; }

        [DisplayName("Công ty")]
        public string CompanyId { get; set; }

        [DisplayName("Ngày đàm phán")]
        public DateTime NgayDamPhan { get; set; }

        [DisplayName("HT giao dịch")]
        public string HinhThucGiaoDich { get; set; }

        [DisplayName("Ngày ký HD")]
        public DateTime NgayKyHopDong { get; set; }

        [DisplayName("Người ký HĐ")]
        public string NguoiKyHopDong { get; set; }

        [DisplayName("Hạn xuất vé")]
        public DateTime HanXuatVe { get; set; }

        [DisplayName("Ngày thanh lý HĐ")]
        public DateTime NgayThanhLyHD { get; set; }

        [DisplayName("Số khách TT")]
        public int SoKhachTT { get; set; }

        [DisplayName("SK trẻ em")]
        public int? SKTreEm { get; set; }

        [DisplayName("Doanh thu TT")]
        public decimal DoanhThuTT { get; set; }

        [DisplayName("Chương trình tour")]
        public string ChuongTrinhTour { get; set; }

        [DisplayName("Nội dung thanh lý HĐ")]
        public string NoiDungThanhLyHD { get; set; }

        [DisplayName("Dịch vụ")]
        public string DichVu { get; set; }

        [DisplayName("Đại lý")]
        public string DaiLy { get; set; }

        [DisplayName("Trạng thái")]
        public string TrangThai { get; set; }

        public DateTime? NgaySua { get; set; }

        [DisplayName("Người sửa")]
        public string NguoiSua { get; set; }

        [DisplayName("Loại tour")]
        public string TenLoaiTour { get; set; }

        [DisplayName("Chi nhánh")]
        public string MaCNTao { get; set; } // chinhanh tao: dmchinhanh: qltour

        public DateTime NgayNhanDuTien { get; set; }

        public string LyDoNhanDu { get; set; }

        [DisplayName("Số HĐ")]
        public string SoHopDong { get; set; }

        public decimal LaiChuaVe { get; set; }
        public decimal LaiGomVe { get; set; }
        public decimal LaiThucTeGomVe { get; set; }

        [DisplayName("Nguồn tour")]
        public string NguonTour { get; set; }

        [DisplayName("File khách đi tour")]
        public string FileKhachDiTour { get; set; }

        [DisplayName("File VMB")]
        public string FileVeMayBay { get; set; }

        [DisplayName("File biên nhận")]
        public string FileBienNhan { get; set; }

        [DisplayName("Người đại diện")]
        public string NguoiDaiDien { get; set; }

        [DisplayName("Đối tác nước ngoài")]
        public string DoiTacNuocNgoai { get; set; }

        [DisplayName("Chi nhánh DH")]
        public string MaCNDH { get; set; }

        [DisplayName("Ngày hủy tour")]
        public DateTime NgayHuyTour { get; set; }

        [DisplayName("Hủy tour")]
        public bool? HuyTour { get; set; }

        [DisplayName("Nội dung hủy")]
        public string NDHuyTour { get; set; }

        [DisplayName("Ghi chú")]
        public string GhiChu { get; set; }

        [DisplayName("Loại tiền")]
        public string LoaiTien { get; set; }

        [DisplayName("Tỷ giá")]
        public decimal? TyGia { get; set; }

        public string LogFile { get; set; }

        public int Invoices { get; set; }

        // DoanhSoTheoSale
        public decimal ChuaThanhLyHopDong { get; set; }
        public decimal DaThanhLyHopDong { get; set; }
        public decimal TongCongTheoTungSale { get; set; }
        public int TongSoKhachTheoSale { get; set; }

        // DoanhSoTheoThiTruong
        public string ThiTruongByNguoiTao { get; set; }
    }
}
