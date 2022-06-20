using System.Collections.Generic;
using ThongKe.Data.DTOs;

namespace ThongKe.Models
{
    public class TourIBDtosGroupByNguoiTaoViewModel
    {
        public IEnumerable<TourIBDTO> TourIBDTOs { get; set; }
        public string NguoiTao { get; set; }
    }
}
