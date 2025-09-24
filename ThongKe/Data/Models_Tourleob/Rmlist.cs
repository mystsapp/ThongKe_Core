using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models_Tourleob
{
    public partial class Rmlist
    {
        public decimal IdRmlist { get; set; }
        public string Supplierid { get; set; }
        public string Sgtcode { get; set; }
        public string Vetourid { get; set; }
        public string Serial { get; set; }
        public string Loaiphong { get; set; }
        public int? Sophong { get; set; }
    }
}
