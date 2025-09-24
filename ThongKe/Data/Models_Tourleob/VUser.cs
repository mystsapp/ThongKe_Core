using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models_Tourleob
{
    public partial class VUser
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
        public bool Trangthai { get; set; }
        public bool? Show { get; set; }
        public bool? Upload { get; set; }
        public string DirPathName { get; set; }
        public string Account { get; set; }
        public string Pass { get; set; }
        public string FlagUrl { get; set; }
        public string DomainNm { get; set; }
        public DateTime Ngaydoimk { get; set; }
        public string Phongban { get; set; }
    }
}
