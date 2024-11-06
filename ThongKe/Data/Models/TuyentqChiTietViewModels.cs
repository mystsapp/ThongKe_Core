using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models
{
    public partial class TuyentqChiTietViewModels
    {
        public long Stt { get; set; }
        public string Chinhanh { get; set; }
        public string Tuyentq { get; set; }
        public string Sgtcode { get; set; }
        public int Vetourid { get; set; }
        public DateTime Batdau { get; set; }
        public DateTime Ketthuc { get; set; }
        public string Dailyxuatve { get; set; }
        public int Sk { get; set; }
        public decimal? Doanhthu { get; set; }
    }
}
