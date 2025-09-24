using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models_Tourleob
{
    public partial class Thecoupon
    {
        public string Soseri { get; set; }
        public DateTime? Ngayhieuluc { get; set; }
        public DateTime? Ngayhethan { get; set; }
        public string Trangthai { get; set; }
        public decimal? Gia { get; set; }
        public DateTime? Ngaygd { get; set; }
        public string Noidung { get; set; }
        public string Codedoan { get; set; }
        public string Vetour { get; set; }
        public string Nguoiban { get; set; }
        public string Daily { get; set; }
        public bool? Quatang { get; set; }
    }
}
