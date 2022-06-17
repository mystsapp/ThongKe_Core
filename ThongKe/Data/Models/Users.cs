using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public string Role { get; set; } // thua
        public bool Doimatkhau { get; set; }
        public DateTime? Ngaydoimk { get; set; }
        public bool Trangthai { get; set; }
        public string Khoi { get; set; }
        public string Nguoitao { get; set; }
        public DateTime? Ngaytao { get; set; }
        public string Nguoicapnhat { get; set; }
        public DateTime? Ngaycapnhat { get; set; }
        public string Nhom { get; set; } // roles

        public string PhongBanQL { get; set; } // danh cho IB
        public string DaiLyQL { get; set; } // danh cho doan ND, OB

        [DisplayName("Phòng ban")]
        [MaxLength(50), Column(TypeName = "nvarchar(50)")]
        public string PhongBanId { get; set; } // in qltour

        [DisplayName("Role")]
        public int RoleId { get; set; }
        //public virtual Role Role { get; set; }
    }
}