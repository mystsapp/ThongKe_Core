using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models_KDIB
{
    public partial class Users
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string HoTen { get; set; }
        public string DienThoai { get; set; }
        public string DaiLy { get; set; }
        public bool TaoTour { get; set; }
        public bool BanVe { get; set; }
        public bool SuaVe { get; set; }
        public bool DongTour { get; set; }
        public bool DcdanhMuc { get; set; }
        public bool SuaTour { get; set; }
        public bool AdminKl { get; set; }
        public bool AdminKd { get; set; }
        public string Email { get; set; }
        public string EmailCc { get; set; }
        public string MaCn { get; set; }
        public bool BanTour { get; set; }
        public bool DoiMk { get; set; }
        public DateTime NgayDoiMk { get; set; }
        public bool TrangThai { get; set; }
        public string NguoiTao { get; set; }
        public DateTime NgayTao { get; set; }
        public string NguoiCapNhat { get; set; }
        public DateTime NgayCapNhat { get; set; }
        public int RoleId { get; set; }
        public string PhongBanId { get; set; }
        public string PhongBans { get; set; }

        public virtual Roles Role { get; set; }
    }
}
