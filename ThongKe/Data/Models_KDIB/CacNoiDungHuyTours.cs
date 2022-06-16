using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models_KDIB
{
    public partial class CacNoiDungHuyTours
    {
        public long Id { get; set; }
        public string NoiDung { get; set; }
        public string NguoiTao { get; set; }
        public DateTime NgayTao { get; set; }
        public string NguoiSua { get; set; }
        public DateTime NgaySua { get; set; }
        public DateTime? NgayXoa { get; set; }
        public string NguoiXoa { get; set; }
        public bool? Xoa { get; set; }
    }
}
