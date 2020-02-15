using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ThongKe.Data.Models
{
    public class ThongKeDoanhThuViewModel
    {
        [Key]
        public string DaiLyXuatVe { get; set; }
        public decimal? DoanhThuHT { get; set; }
        public decimal? DoanhThuTT { get; set; }
    }
}
