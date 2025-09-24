using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models_Tourleob
{
    public partial class CtdatcocDel
    {
        public decimal Idctdatcoc { get; set; }
        public decimal Iddatcoc { get; set; }
        public decimal Idvetour { get; set; }
        public string Httt { get; set; }
        public string Chungtugoc { get; set; }
        public decimal Sotienct { get; set; }
        public string Ghichu { get; set; }
        public DateTime? Capnhat { get; set; }
        public string Nguoicapnhat { get; set; }
        public string Computer { get; set; }
    }
}
