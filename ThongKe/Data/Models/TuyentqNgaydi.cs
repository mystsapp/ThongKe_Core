using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models
{
    public partial class TuyentqNgaydi
    {
        public long Stt { get; set; }
        public string Chinhanh { get; set; }
        public string Tuyentq { get; set; }
        public int? Khachht { get; set; }
        public decimal? Thucthuht { get; set; }
        public int? Khachcu { get; set; }
        public decimal? Thucthucu { get; set; }
    }
}
