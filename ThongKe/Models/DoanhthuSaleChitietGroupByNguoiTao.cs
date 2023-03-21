using System.Collections.Generic;
using ThongKe.Data.Models;

namespace ThongKe.Models
{
    public class DoanhthuSaleChitietGroupByNguoiTao
    {
        public IEnumerable<DoanhThuSaleChiTietAll> DoanhthuSaleChitiets { get; set; }
        public string NguoiTao { get; set; }
    }
}
