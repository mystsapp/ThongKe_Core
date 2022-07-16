using System.Collections.Generic;
using ThongKe.Data.DTOs;
using ThongKe.Data.Models;
using ThongKe.Data.Models_KDND;
using ThongKe.Data.Models_QLTour;
using ThongKe.Models.TourTheoNgay;

namespace ThongKe.Models
{
    public class BaoCaoViewModel
    {
        public IEnumerable<Dmchinhanh> Dmchinhanhs { get; set; }
        public IEnumerable<Company> Companies { get; set; }
        public IEnumerable<KhoiViewModel> Khois_KD { get; set; }
        public IEnumerable<TourIBDTO> TourIBDTOs { get; internal set; }
        public IEnumerable<TourNDDTO> TourNDDTOs { get; internal set; }
        public IEnumerable<TourOBDTO> TourOBDTOs { get; internal set; }
        public IEnumerable<TourIBDtosGroupByNguoiTaoViewModel> TourIBDtosGroupByNguoiTaos { get; set; }
        public IEnumerable<TourNDDtosGroupByNguoiTaoViewModel> TourNDDtosGroupByNguoiTaos { get; set; }
        public IEnumerable<TourOBDtosGroupByNguoiTaoViewModel> TourOBDtosGroupByNguoiTaos { get; internal set; }
        

        // theo thang
        public IEnumerable<TourBaoCaoTheoThangViewModel> TourBaoCaoTheoThangs1_IB { get; set; }
        public IEnumerable<ListViewModel> Thangs { get; set; }
        public IEnumerable<TourBaoCaoTheoThangViewModel> TourBaoCaoTheoThangs2_IB { get; set; }

        public IEnumerable<TourBaoCaoTheoThangViewModel> TourBaoCaoTheoThangs1_ND { get; internal set; }
        public IEnumerable<TourBaoCaoTheoThangViewModel> TourBaoCaoTheoThangs2_ND { get; internal set; }

        public IEnumerable<TourBaoCaoTheoThangViewModel> TourBaoCaoTheoThangs1_OB { get; internal set; }
        public IEnumerable<TourBaoCaoTheoThangViewModel> TourBaoCaoTheoThangs2_OB { get; internal set; }
        // theo thang

        // theo ngay di
        public IEnumerable<Tourkind> Tourkinds { get; set; } // qltour
        public IEnumerable<Loaitour> Loaitours { get; set; } // khachdoanND
        public TourTheoNgay_IB TourTheoNgay_IB { get; set; }
        public TourTheoNgay_ND TourTheoNgay_ND { get; set; }
        public TourTheoNgay_OB TourTheoNgay_OB { get; set; }
        // theo ngay di

        // theo thi truong
        public IEnumerable<Phongban> Phongbans { get; set; }
        // theo thi truong

        public decimal? TongCong { get; set; }
        public int? TongSK { get; set; }
        public string Khoi { get; set; }
        
    }
}
