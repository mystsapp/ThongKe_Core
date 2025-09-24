using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models_Tourleob
{
    public partial class VChinhanh
    {
        public int Id { get; set; }
        public string Chinhanh { get; set; }
        public string Tencn { get; set; }
        public string Diachi { get; set; }
        public string Thanhpho { get; set; }
        public string Dienthoai { get; set; }
        public string Fax { get; set; }
        public string Masothue { get; set; }
        public bool Trangthai { get; set; }
        public string DirPathName { get; set; }
        public string FlagUrl { get; set; }
        public string Account { get; set; }
        public string Pass { get; set; }
        public string DomainNm { get; set; }
    }
}
