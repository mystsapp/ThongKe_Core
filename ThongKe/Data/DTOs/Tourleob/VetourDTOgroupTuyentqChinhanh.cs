using System;
using System.Collections.Generic;

namespace ThongKe.Data.DTOs.Tourleob
{
    public class VetourDTOgroupTuyentqChinhanh
    {
        public string TuyenTq { get; set; }
        public string Chinhanh { get; set; }
        public string Sgtcode { get; set; }
        public DateTime Batdau { get; set; }
        public DateTime Ketthuc { get; set; }
        //public int SoKhach { get; set; }
        //public decimal DoanhThu { get; set; }
        public List<VetourDTO> VetourDTOs { get; set; }
    }
}
