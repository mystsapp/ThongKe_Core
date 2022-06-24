﻿using System.Collections.Generic;
using ThongKe.Data.DTOs;
using ThongKe.Data.Models;
using ThongKe.Data.Models_QLTour;

namespace ThongKe.Models
{
    public class BaoCaoViewModel
    {
        public IEnumerable<Dmchinhanh> Dmchinhanhs { get; set; }
        public IEnumerable<KhoiViewModel> Khois_KD { get; set; }
        public IEnumerable<TourIBDTO> TourIBDTOs { get; internal set; }
        public IEnumerable<TourNDDTO> TourNDDTOs { get; internal set; }
        public IEnumerable<TourOBDTO> TourOBDTOs { get; internal set; }
        public IEnumerable<TourIBDtosGroupByNguoiTaoViewModel> TourIBDtosGroupByNguoiTaos { get; set; }
        public IEnumerable<TourNDDtosGroupByNguoiTaoViewModel> TourNDDtosGroupByNguoiTaos { get; set; }
        public IEnumerable<TourOBDtosGroupByNguoiTaoViewModel> TourOBDtosGroupByNguoiTaos { get; internal set; }
        public decimal? TongCong { get; set; }
        public int? TongSK { get; set; }
        public string Khoi { get; set; }
        
    }
}
