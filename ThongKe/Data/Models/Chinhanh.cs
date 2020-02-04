using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models
{
    public partial class Chinhanh
    {
        public int Id { get; set; }
        public string Chinhanh1 { get; set; }
        public string Tencn { get; set; }
        public string Diachi { get; set; }
        public string Thanhpho { get; set; }
        public string Dienthoai { get; set; }
        public string Fax { get; set; }
        public string Masothue { get; set; }
        public bool? Trangthai { get; set; }
        public string Nhom { get; set; }
    }
}
