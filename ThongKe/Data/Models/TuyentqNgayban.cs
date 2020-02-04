using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models
{
    public partial class TuyentqNgayban
    {
        public long Stt { get; set; }
        public string Chinhanh { get; set; }
        public string Tuyentq { get; set; }
        public int? Sokhach { get; set; }
        public decimal? Tongtien { get; set; }
        public decimal? Thucthu { get; set; }
    }
}
