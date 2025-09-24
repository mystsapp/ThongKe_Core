using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models_Tourleob
{
    public partial class Supplier
    {
        public string Code { get; set; }
        public string Codecn { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Nation { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string Taxcode { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }
        public string Field { get; set; }
        public string Paymentcode { get; set; }
        public decimal? Room { get; set; }
        public int? Level { get; set; }
        public string Website { get; set; }
        public bool? Trangthai { get; set; }
    }
}
