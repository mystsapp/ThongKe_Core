using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models
{
    public partial class Roles
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public string NguoiTao { get; set; }
        public DateTime? NgayTao { get; set; }
        public string NguoiSua { get; set; }
        public DateTime? NgaySua { get; set; }
        public string ChiNhanhQl { get; set; }
    }
}
