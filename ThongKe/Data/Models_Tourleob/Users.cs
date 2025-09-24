using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models_Tourleob
{
    public partial class Users
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Dienthoai { get; set; }
        public string Daily { get; set; }
        public bool Taotour { get; set; }
        public bool Banve { get; set; }
        public bool Suave { get; set; }
        public bool Dongtour { get; set; }
        public bool Dcdanhmuc { get; set; }
        public bool Suatour { get; set; }
        public bool Adminkl { get; set; }
        public bool Adminkd { get; set; }
        public string Email { get; set; }
        public string Emailcc { get; set; }
        public string Chinhanh { get; set; }
        public string Bantour { get; set; }
        public string Role { get; set; }
        public bool Doimk { get; set; }
        public DateTime Ngaydoimk { get; set; }
        public bool? Trangthai { get; set; }
        public string Nguoitao { get; set; }
        public DateTime Ngaytao { get; set; }
        public string Nguoicapnhat { get; set; }
        public DateTime? Ngaycapnhat { get; set; }
    }
}
