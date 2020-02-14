using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ThongKe.Data.Models;

namespace ThongKe.Models
{
    public class LoginViewModel
    {
        [Key]
        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "Vui lòng nhập username")]
        public string Username { get; set; }
        public string Mact { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập password")]
        public string Password { get; set; }

        //public Users Users { get; set; }
        //public string Hoten { get; set; }
        //public string Dienthoai { get; set; }
        //public string Email { get; set; }
        //public string Macn { get; set; }
        //public string RoleId { get; set; }
        public bool Trangthai { get; set; }
        public bool Doimk { get; set; }
        //public DateTime? Ngaydoimk { get; set; }
        //public string Maphong { get; set; }
        //public string Macode { get; set; }
    }
}
