using System;
using System.ComponentModel.DataAnnotations;

namespace ThongKe.Data.Models
{
    public partial class Users
    {
        [Required(ErrorMessage = "Username không được bỏ trống.")]
        [MaxLength(50, ErrorMessage = "Không vượt qua 50 ký tự.")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password không được bỏ trống.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Hoten không được bỏ trống.")]
        [MaxLength(50, ErrorMessage = "Không vượt qua 50 ký tự.")]
        public string Hoten { get; set; }

        public string Daily { get; set; }
        public string Chinhanh { get; set; }
        public string Role { get; set; }
        public bool Doimatkhau { get; set; }
        public DateTime? Ngaydoimk { get; set; }
        public bool Trangthai { get; set; }
        public string Khoi { get; set; }
        public string Nguoitao { get; set; }
        public DateTime? Ngaytao { get; set; }
        public string Nguoicapnhat { get; set; }
        public DateTime? Ngaycapnhat { get; set; }
        public string Nhom { get; set; }
    }
}