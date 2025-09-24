using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models_Tourleob
{
    public partial class Khachsan
    {
        public decimal Idks { get; set; }
        public string Sgtcode { get; set; }
        public int? Stt { get; set; }
        public DateTime Checkin { get; set; }
        public DateTime Checkout { get; set; }
        public string Maks { get; set; }
        public string Tenks { get; set; }
        public string Loaigia { get; set; }
        public string Tinhtp { get; set; }
        public int? Sophong { get; set; }
        public string Ghichu { get; set; }
        public DateTime? Ngaytao { get; set; }
        public string Dieuhanh { get; set; }
    }
}
