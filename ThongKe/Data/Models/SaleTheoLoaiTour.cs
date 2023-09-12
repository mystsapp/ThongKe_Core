using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ThongKe.Data.Models
{
    public class SaleTheoLoaiTour
    {
        [Key]
        public long Stt { get; set; }
        public string Chinhanh { get; set; }
        public string Tuyentq { get; set; }
        public decimal? Doanhso { get; set; }
        public decimal? Thucthu { get; set; }
    }
}
