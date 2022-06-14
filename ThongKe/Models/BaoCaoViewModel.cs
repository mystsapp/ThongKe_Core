using System.Collections.Generic;
using ThongKe.Data.DTOs;
using ThongKe.Data.Models;

namespace ThongKe.Models
{
    public class BaoCaoViewModel
    {
        public IEnumerable<Chinhanh> Dmchinhanhs { get; set; }
        public IEnumerable<KhoiViewModel> Khois_KD { get; set; }
        public IEnumerable<TourIBDTO> TourIBDTOs { get; internal set; }
    }
}
