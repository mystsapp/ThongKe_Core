using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models
{
    public partial class DoanhthuSaleChitiet
    {
        public long Stt { get; set; }
        public string Chinhanh { get; set; }
        public string Sgtcode { get; set; }
        public string Tuyentq { get; set; }
        public string Tenkhach { get; set; }
        public int? Chiemcho { get; set; }
        public decimal? Doanhthu { get; set; }
        public decimal? Thucthu { get; set; }
        public string Nguoixuatve { get; set; }
    }
}
