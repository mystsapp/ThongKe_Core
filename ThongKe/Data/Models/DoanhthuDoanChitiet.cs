using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models
{
    public partial class DoanhthuDoanChitiet
    {
        public long Id { get; set; }
        public int? Vetourid { get; set; }
        public int? Stt { get; set; }
        public string Serial { get; set; }
        public string Tenkhach { get; set; }
        public string Diachi { get; set; }
        public string Diemdon { get; set; }
        public decimal? Giave { get; set; }
        public decimal? Thucthu { get; set; }
        public decimal? Congno { get; set; }
        public string Ghichu { get; set; }
    }
}
