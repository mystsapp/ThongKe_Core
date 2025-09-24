using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models_Tourleob
{
    public partial class Appuser
    {
        public long Id { get; set; }
        public string Mact { get; set; }
        public string Chuongtrinh { get; set; }
        public string Link { get; set; }
        public string Mota { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool? Trangthai { get; set; }
        public bool Doimk { get; set; }
        public DateTime Ngaydoimk { get; set; }
    }
}
