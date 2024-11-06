using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models
{
    public partial class Users
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Hoten { get; set; }
        public string Daily { get; set; }
        public string Chinhanh { get; set; }
        public bool Doimatkhau { get; set; }
        public DateTime? Ngaydoimk { get; set; }
        public bool Trangthai { get; set; }
        public string Khoi { get; set; }
        public string Nguoitao { get; set; }
        public DateTime? Ngaytao { get; set; }
        public string Nguoicapnhat { get; set; }
        public DateTime? Ngaycapnhat { get; set; }
        public string Nhom { get; set; }
        public string DaiLyQl { get; set; }
        public string PhongBanQl { get; set; }
        public string PhongBanId { get; set; }
        public int RoleId { get; set; }
        public string Role { get; set; }
    }
}
