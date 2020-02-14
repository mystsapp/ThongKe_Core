using System.ComponentModel.DataAnnotations;

namespace ThongKe.Models
{
    public class ChangePassViewModel
    {
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập password cũ")]
        public string Password { get; set; }

        [Display(Name = "Mật khẩu mới")]
        [Required(ErrorMessage = "Vui lòng nhập password mới")]
        public string NewPassword { get; set; }

        [Display(Name = "Mật khẩu mới")]
        [Required(ErrorMessage = "Vui lòng nhập lại password mới")]
        [Compare("NewPassword", ErrorMessage = "The new passord and confirm password do not match.")]
        public string Confirmpassword { get; set; }

        public string strUrl { get; set; }
    }
}