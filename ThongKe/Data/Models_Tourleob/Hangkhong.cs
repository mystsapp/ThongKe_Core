using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models_Tourleob
{
    public partial class Hangkhong
    {
        public decimal Id { get; set; }
        public string Sgtcode { get; set; }
        public string Mancc { get; set; }
        public string Tenncc { get; set; }
        public string Iddv { get; set; }
        public string Hanhtrinh { get; set; }
        public string Codebooking { get; set; }
        public string Loaibooking { get; set; }
        public int Chococ1 { get; set; }
        public decimal Tiencoc1 { get; set; }
        public int Chococ2 { get; set; }
        public decimal Tiencoc2 { get; set; }
        public int Chococ3 { get; set; }
        public decimal Tiencoc3 { get; set; }
        public int Sochoxuatve { get; set; }
        public decimal Tiencocphat { get; set; }
        public decimal Tiencochoan { get; set; }
        public string Ghichu { get; set; }
        public string Logfilehk { get; set; }
    }
}
