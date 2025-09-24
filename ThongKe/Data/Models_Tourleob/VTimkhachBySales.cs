using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models_Tourleob
{
    public partial class VTimkhachBySales
    {
        public string Sgtcode { get; set; }
        public int VetourId { get; set; }
        public int Stt { get; set; }
        public string Tenkhach { get; set; }
        public string Diachi { get; set; }
        public string Dienthoai { get; set; }
        public string Nguoixuatve { get; set; }
        public DateTime Batdau { get; set; }
        public DateTime Ketthuc { get; set; }
    }
}
