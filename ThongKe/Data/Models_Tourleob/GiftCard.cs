using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models_Tourleob
{
    public partial class GiftCard
    {
        public int Id { get; set; }
        public string GiftCode { get; set; }
        public DateTime? HieuLucCode { get; set; }
        public int? TinhTrang { get; set; }
        public string CodeDoan { get; set; }
        public string HanhTrinh { get; set; }
        public DateTime? BatDau { get; set; }
        public DateTime? KetThuc { get; set; }
        public string SaleText { get; set; }
        public decimal HoaMai { get; set; }
    }
}
