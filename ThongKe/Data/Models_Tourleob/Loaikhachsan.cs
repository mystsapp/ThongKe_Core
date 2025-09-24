using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models_Tourleob
{
    public partial class Loaikhachsan
    {
        public decimal Id { get; set; }
        public string Sgtcode { get; set; }
        public string Hotel { get; set; }
        public int Sophong { get; set; }
        public decimal Giatour { get; set; }
        public string Ghichu { get; set; }
    }
}
