using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models_Tourleob
{
    public partial class Vetour
    {
        public decimal Id { get; set; }
        public string Sgtcode { get; set; }
        public int VetourId { get; set; }
        public DateTime Ngaytao { get; set; }
        public string Makh { get; set; }
        public string Tencoquan { get; set; }
        public string Tenkhach { get; set; }
        public string Diachi { get; set; }
        public string Quan { get; set; }
        public string Thanhpho { get; set; }
        public string Dienthoai { get; set; }
        public string Email { get; set; }
        public string Diemdon { get; set; }
        public string Ghichuvetour { get; set; }
        public decimal Dichvukhac { get; set; }
        public string Ghichudvk { get; set; }
        public string Yeucauks { get; set; }
        public string Serial { get; set; }
        public DateTime? Ngayxuatve { get; set; }
        public string Nguoixuatve { get; set; }
        public string Dailyxuatve { get; set; }
        public decimal Giatour { get; set; }
        public decimal Hoahong { get; set; }
        public DateTime? Ngaychihh { get; set; }
        public string Nguoinhanhh { get; set; }
        public string Phieuchihh { get; set; }
        public string Cachtinhhh { get; set; }
        public string Nguoichihh { get; set; }
        public string Huyve { get; set; }
        public DateTime? Ngayhuyve { get; set; }
        public string Nguoihuyve { get; set; }
        public string Dailyhuyve { get; set; }
        public string Noidunghuyve { get; set; }
        public string Idchuyenve { get; set; }
        public decimal Tienhoan { get; set; }
        public decimal Lephihuy { get; set; }
        public int Tuhuy { get; set; }
        public string Kenhgd { get; set; }
        public string Kenhtt { get; set; }
        public string Magdonline { get; set; }
        public string Thetindung { get; set; }
        public decimal Giamgia { get; set; }
        public string Lydogiamgia { get; set; }
        public int Chiemcho { get; set; }
        public DateTime? Ngaythutien { get; set; }
        public string Nguoithu { get; set; }
        public DateTime? Capnhat { get; set; }
        public string Computer { get; set; }
        public string Logfile { get; set; }
        public bool Inbaohiem { get; set; }
        public bool Dongy { get; set; }
    }
}
