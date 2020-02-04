using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models
{
    public partial class QuayNgayBan
    {
        public long Stt { get; set; }
        public string Dailyxuatve { get; set; }
        public string Chinhanh { get; set; }
        public int? Sokhach { get; set; }
        public decimal? Doanhso { get; set; }
        public decimal? Doanhthu { get; set; }
    }
}
