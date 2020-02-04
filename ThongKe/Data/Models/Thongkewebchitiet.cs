using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models
{
    public partial class Thongkewebchitiet
    {
        public long Stt { get; set; }
        public string Sgtcode { get; set; }
        public string Hanhtrinh { get; set; }
        public DateTime? Ngaydi { get; set; }
        public DateTime? Ngayve { get; set; }
        public string Tenkhach { get; set; }
        public string Serial { get; set; }
        public string Huyve { get; set; }
        public int? Sokhach { get; set; }
        public decimal? Doanhso { get; set; }
        public string Nguoixuatve { get; set; }
        public string Dailyxuatve { get; set; }
        public string Kenhgd { get; set; }
        public string Chinhanh { get; set; }
        public string Trangthai { get; set; }
        public DateTime? Ngaytao { get; set; }
    }
}
