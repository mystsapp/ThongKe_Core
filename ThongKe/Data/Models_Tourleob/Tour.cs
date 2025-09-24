using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models_Tourleob
{
    public partial class Tour
    {
        public decimal Idtour { get; set; }
        public string Sgtcode { get; set; }
        public DateTime Ngaytao { get; set; }
        public bool? Khachle { get; set; }
        public string Makh { get; set; }
        public string Tuyentq { get; set; }
        public string Chudetour { get; set; }
        public DateTime Batdau { get; set; }
        public DateTime Ketthuc { get; set; }
        public int Socho { get; set; }
        public string Ghichu { get; set; }
        public string Cttour { get; set; }
        public string Ksdukien { get; set; }
        public string Nhdukien { get; set; }
        public string Vanchuyen { get; set; }
        public string Diemtq { get; set; }
        public decimal Dtnuocngoainl { get; set; }
        public decimal Dtnuocngoaite { get; set; }
        public decimal Dtnuocngoaieb { get; set; }
        public decimal Giamgia { get; set; }
        public bool Online { get; set; }
        public string Ghichuonline { get; set; }
        public DateTime? Hanxuatvmb { get; set; }
        public DateTime? Hanlamvisa { get; set; }
        public DateTime? Dongtour { get; set; }
        public string Nguoitaotour { get; set; }
        public DateTime? Huytour { get; set; }
        public string Nguoihuy { get; set; }
        public string Lydohuy { get; set; }
        public string Noikhoihanh { get; set; }
        public string Chinhanh { get; set; }
        public string Chinhanhdh { get; set; }
        public int? Choconlai { get; set; }
        public string Loaitour { get; set; }
        public string Noidungtinnhan { get; set; }
        public string Dienthoaihd { get; set; }
        public DateTime? Ngayhopdoan { get; set; }
        public string Logfile { get; set; }
    }
}
