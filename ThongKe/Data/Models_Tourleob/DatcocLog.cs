using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models_Tourleob
{
    public partial class DatcocLog
    {
        public decimal Id { get; set; }
        public decimal? Idvetour { get; set; }
        public int Stt { get; set; }
        public string Biennhan { get; set; }
        public DateTime? Ngaydatcoc { get; set; }
        public string Nguoilambn { get; set; }
        public string Daily { get; set; }
        public string Tenkhach { get; set; }
        public string Diachi { get; set; }
        public string Dienthoai { get; set; }
        public string Noidung { get; set; }
        public decimal Sotien { get; set; }
        public DateTime? Ngaythu { get; set; }
        public string Nguoithu { get; set; }
        public string Type { get; set; }
        public DateTime Ngaycapnhat { get; set; }
        public string Computer { get; set; }
    }
}
