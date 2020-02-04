using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThongKe.Data.Models;

namespace ThongKe.Models
{
    public class DoanhThuSaleTuyenViewModel
    {
        public IEnumerable<DoanhthuSaleTuyen> DoanhthuSaleTuyens { get; set; }
        public IEnumerable<TuyenThamQuanViewModel> tuyenThamQuanViewModels { get; set; }

    }
}
