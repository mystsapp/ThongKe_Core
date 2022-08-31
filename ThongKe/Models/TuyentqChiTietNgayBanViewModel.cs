using System;
using System.ComponentModel.DataAnnotations;

namespace ThongKe.Models
{
    public class TuyentqChiTietNgayBanViewModel
    {
        [Key]
        public long stt { get; set; }
        public string chinhanh { get; set; }
        public DateTime batdau { get; set; }
        public DateTime ketthuc { get; set; }
        public int sk { get; set; }
        public string sgtcode { get; set; }
        public int vetourid { get; set; }
        public string tuyentq { get; set; }
        public DateTime? ngayxuatve { get; set; }
        public string nguoixuatve { get; set; }
        public string dailyxuatve { get; set; }
        public decimal? doanhso { get; set; }
    }
}
