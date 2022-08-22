using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ThongKe.Models
{
    public class TuyentqChiTietViewModel
    {
        [Key]
        public long stt { get; set; }
        public DateTime? ngaytao { get; set; }
        public string chinhanh { get; set; }
        public string tuyentq { get; set; }
        public string sgtcode { get; set; }
        public int vetourid { get; set; }
        public DateTime batdau { get; set; }
        public DateTime ketthuc { get; set; }
        public string dailyxuatve { get; set; }
        public int sk { get; set; }
        public decimal? doanhthu { get; set; }
    }
}
