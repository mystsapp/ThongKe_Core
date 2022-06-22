using System.Collections.Generic;
using ThongKe.Data.DTOs;

namespace ThongKe.Models
{
    public class TourNDDtosGroupByNguoiTaoViewModel
    {
        public IEnumerable<TourNDDTO> TourNDDTOs { get; set; }
        public string NguoiTao { get; set; }
    }
}
