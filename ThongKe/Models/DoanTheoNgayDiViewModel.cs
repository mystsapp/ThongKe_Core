using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThongKe.Data.Models;

namespace ThongKe.Models
{
    public class DoanTheoNgayDiViewModel
    {
        public IEnumerable<DoanhthuDoanNgayDi> DoanhthuDoanNgayDis { get; set; }
        public List<ChiNhanhToReturnViewModel> chiNhanhToReturnViewModels { get; set; }
        public List<KhoiViewModel> khoiViewModels { get; set; }
        public DoanTheoNgayDiViewModel()
        {
            chiNhanhToReturnViewModels = new List<ChiNhanhToReturnViewModel>();
            khoiViewModels = new List<KhoiViewModel>();
        }
    }
}
