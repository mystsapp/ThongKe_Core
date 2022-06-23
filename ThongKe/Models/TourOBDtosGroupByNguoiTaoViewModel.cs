using System.Collections.Generic;
using ThongKe.Data.DTOs;

namespace ThongKe.Models
{
    public class TourOBDtosGroupByNguoiTaoViewModel
    {
        public IEnumerable<TourOBDTO> TourOBDTOs { get; set; }
        public string NguoiTao { get; set; }
    }
}
