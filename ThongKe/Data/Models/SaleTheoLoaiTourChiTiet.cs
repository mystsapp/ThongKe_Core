using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ThongKe.Data.Models
{
    public partial class SaleTheoLoaiTourChiTiet
    {
        [Key]
        public long Stt { get; set; }
        public string Chinhanh { get; set; }
        public DateTime? Batdau { get; set; }
        public DateTime? Ketthuc { get; set; }
        public int SK { get; set; }
        public string Sgtcode { get; set; }
        public decimal Vetourid { get; set; }
        public string Tuyentq { get; set; }
        public DateTime? Ngayxuatve { get; set; }
        public string Nguoixuatve { get; set; }
        public string Dailyxuatve { get; set; }
        public decimal? Doanhso { get; set; }
        
    }
}
