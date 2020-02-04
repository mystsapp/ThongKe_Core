using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models
{
    public partial class DoanhthuQuayChitiet
    {
        public long Stt { get; set; }
        public string Chinhanh { get; set; }
        public string Sgtcode { get; set; }
        public string Serial { get; set; }
        public string Tenkhach { get; set; }
        public string Hanhtrinh { get; set; }
        public string Ngaydi { get; set; }
        public string Ngayve { get; set; }
        public int Sokhach { get; set; }
        public decimal? Giave { get; set; }
        public string Nguoiban { get; set; }
    }
}
