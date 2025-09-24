using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models_Tourleob
{
    public partial class Datcoc
    {
        public decimal Iddatcoc { get; set; }
        public decimal Idvetour { get; set; }
        public int Stt { get; set; }
        public string Biennhan { get; set; }
        public DateTime Ngaydatcoc { get; set; }
        public string Nguoilambn { get; set; }
        public string Daily { get; set; }
        public string Tenkhach { get; set; }
        public string Diachi { get; set; }
        public string Dienthoai { get; set; }
        public string Noidung { get; set; }
        public decimal Sotien { get; set; }
        public DateTime? Ngaythu { get; set; }
        public string Nguoithu { get; set; }
        public DateTime? Ngayhuy { get; set; }
        public string Nguoihuy { get; set; }
        public string Lydohuydc { get; set; }
        public string Computer { get; set; }
    }
}
