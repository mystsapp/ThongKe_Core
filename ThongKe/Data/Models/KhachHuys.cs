using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models
{
    public partial class KhachHuys
    {
        public long Stt { get; set; }
        public string Tenkhach { get; set; }
        public string Sgtcode { get; set; }
        public int Vetourid { get; set; }
        public string Tuyentq { get; set; }
        public DateTime Batdau { get; set; }
        public DateTime Ketthuc { get; set; }
        public decimal? Giatour { get; set; }
        public string Nguoihuyve { get; set; }
        public string Dailyhuyve { get; set; }
        public string Chinhanh { get; set; }
        public DateTime Ngayhuyve { get; set; }
    }
}
