using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThongKe.Data.Models
{
    public class Role
    {
        public int Id { get; set; }

        [DisplayName("Role name")]
        [MaxLength(50), Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "*")]
        public string RoleName { get; set; }

        [DisplayName("Miêu tả")]
        [MaxLength(250), Column(TypeName = "nvarchar(250)")]
        public string Description { get; set; }
        [DisplayName("Chi nhánh")]
        [MaxLength(250), Column(TypeName = "nvarchar(250)")]
        //[Required(ErrorMessage = "Chi nhánh không được trống")]
        public string ChiNhanhQL { get; set; }

        [DisplayName("Người tạo")]
        [MaxLength(50), Column(TypeName = "varchar(50)")]
        public string NguoiTao { get; set; }

        public DateTime? NgayTao { get; set; }

        [DisplayName("Người sửa")]
        [MaxLength(50), Column(TypeName = "varchar(50)")]
        public string NguoiSua { get; set; }

        public DateTime? NgaySua { get; set; }
    }
}
