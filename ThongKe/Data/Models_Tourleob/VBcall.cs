using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models_Tourleob
{
    public partial class VBcall
    {
        public string Chinhanh { get; set; }
        public string Sgtcode { get; set; }
        public string Tuyentq { get; set; }
        public DateTime Batdau { get; set; }
        public DateTime Ketthuc { get; set; }
        public string Serial { get; set; }
        public int? Sokhach { get; set; }
        public string Dailyxuatve { get; set; }
        public int VetourId { get; set; }
        public DateTime? Ngayxuatve { get; set; }
        public decimal Dichvukhac { get; set; }
        public decimal Giamgia { get; set; }
        public decimal Giatour { get; set; }
        public string Nguoixuatve { get; set; }
        public string Tenkhach { get; set; }
        public string Ghichuvetour { get; set; }
        public decimal? Doanhthu { get; set; }
        public decimal? Thucthu { get; set; }
    }
}
