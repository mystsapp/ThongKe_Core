using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models
{
    public partial class Dmdaily
    {
        public int Id { get; set; }
        public string Daily { get; set; }
        public string TenDaily { get; set; }
        public string Diachi { get; set; }
        public string Dienthoai { get; set; }
        public string Fax { get; set; }
        public string Chinhanh { get; set; }
        public bool? Trangthai { get; set; }
    }
}
