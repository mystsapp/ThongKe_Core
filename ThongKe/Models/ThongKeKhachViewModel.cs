using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ThongKe.Models
{
    public class ThongKeKhachViewModel
    {
        [Key]
        public string DaiLyXuatVe { get; set; }
        public int? SoKhachHT { get; set; }
        public int? SoKhachTT { get; set; }
    }
}
