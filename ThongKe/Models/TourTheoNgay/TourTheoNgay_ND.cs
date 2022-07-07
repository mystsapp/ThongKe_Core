using System.Collections.Generic;
using ThongKe.Data.DTOs;

namespace ThongKe.Models.TourTheoNgay
{
    public class TourTheoNgay_ND
    {
        public IEnumerable<TourNDDTO> TourNDDTOs { get; set; }

        public int TongSK { get; set; }
        public decimal TongDS { get; set; }

        public int TongSKCacDoanDaThanhLy { get; set; }
        public decimal TongDSCacDoanDaThanhLy { get; set; }

        public int TongSKCacDoanChuaThanhLy { get; set; }
        public decimal TongDSCacDoanChuaThanhLy { get; set; }

        public int TongSKCacDoanChuaKyHD { get; set; }
        public decimal TongDSCacDoanChuaKyHD { get; set; }
    }
}
