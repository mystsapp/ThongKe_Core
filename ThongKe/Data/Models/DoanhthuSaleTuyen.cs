using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models
{
    public partial class DoanhthuSaleTuyen
    {
        public long Stt { get; set; }
        public string Nguoixuatve { get; set; }
        public string Chinhanh { get; set; }
        public string Tuyentq { get; set; }
        public decimal? Doanhso { get; set; }
        public decimal? Thucthu { get; set; }
    }
}
