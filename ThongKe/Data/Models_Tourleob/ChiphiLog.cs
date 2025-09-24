using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models_Tourleob
{
    public partial class ChiphiLog
    {
        public decimal Id { get; set; }
        public string Sgtcode { get; set; }
        public string Mancc { get; set; }
        public string Tenncc { get; set; }
        public string Iddv { get; set; }
        public decimal? Tienmat { get; set; }
        public decimal? Ngoaite { get; set; }
        public string Loaitien { get; set; }
        public decimal? Tigia { get; set; }
        public decimal? Tienvnd { get; set; }
        public int? Sokhach { get; set; }
        public string Noidung { get; set; }
        public string Ghichu { get; set; }
        public string Nguoinhap { get; set; }
        public DateTime? Ngaynhap { get; set; }
        public string Type { get; set; }
        public string Computer { get; set; }
    }
}
