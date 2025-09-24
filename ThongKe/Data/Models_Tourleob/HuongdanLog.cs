using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models_Tourleob
{
    public partial class HuongdanLog
    {
        public decimal? Id { get; set; }
        public string Sgtcode { get; set; }
        public string Stt { get; set; }
        public string Tenhd { get; set; }
        public bool? Phai { get; set; }
        public DateTime? Ngaysinh { get; set; }
        public string Hochieu { get; set; }
        public string Dienthoai { get; set; }
        public DateTime? Hieuluchc { get; set; }
        public string Quoctich { get; set; }
        public string Phongks { get; set; }
        public string Ghichu { get; set; }
        public bool? Vemaybay { get; set; }
        public string Nguoicapnhat { get; set; }
        public DateTime? Ngaycapnhat { get; set; }
        public string Loaicapnhat { get; set; }
        public string Maytinh { get; set; }
    }
}
