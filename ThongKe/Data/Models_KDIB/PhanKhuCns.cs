using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models_KDIB
{
    public partial class PhanKhuCns
    {
        public int RoleId { get; set; }
        public string ChiNhanhs { get; set; }
        public string NguoiTao { get; set; }
        public DateTime NgayTao { get; set; }
        public string NguoiSua { get; set; }
        public DateTime NgaySua { get; set; }

        public virtual Roles Role { get; set; }
    }
}
