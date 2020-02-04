using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models
{
    public partial class DoanhthuDoanNgayDi
    {
        public long Stt { get; set; }
        public string Sgtcode { get; set; }
        public string Tuyentq { get; set; }
        public DateTime? Batdau { get; set; }
        public DateTime? Ketthuc { get; set; }
        public int? Sokhach { get; set; }
        public decimal? Doanhthu { get; set; }
    }
}
