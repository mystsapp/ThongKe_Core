using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models_Tourleob
{
    public partial class VwRoomListExcel
    {
        public string Sgtcode { get; set; }
        public string Tenks { get; set; }
        public DateTime Checkin { get; set; }
        public DateTime Checkout { get; set; }
        public string Tourleader { get; set; }
        public string Mobile { get; set; }
        public string Serial { get; set; }
        public string Tenkhach { get; set; }
        public bool? Gioitinh { get; set; }
        public DateTime Ngaysinh { get; set; }
        public string Hochieu { get; set; }
        public DateTime Hieuluchc { get; set; }
        public string Loaiphong { get; set; }
        public string Ghichu { get; set; }
    }
}
