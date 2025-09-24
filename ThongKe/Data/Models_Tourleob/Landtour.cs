using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models_Tourleob
{
    public partial class Landtour
    {
        public string Sgtcode { get; set; }
        public int Order { get; set; }
        public string Hanhtrinh { get; set; }
        public string Loai1 { get; set; }
        public string Loai2 { get; set; }
        public decimal Giavenl { get; set; }
        public decimal Giavete { get; set; }
        public decimal Giaveeb { get; set; }
        public int Socho { get; set; }
        public int Choconlai { get; set; }
        public string Ghichu { get; set; }
        public int Sophong { get; set; }
        public double Phongconlai { get; set; }
    }
}
