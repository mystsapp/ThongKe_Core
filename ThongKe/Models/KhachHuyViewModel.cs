using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThongKe.Models
{
    public class KhachHuyViewModel
    {
        public IEnumerable<KhachHuy> KhachHuys { get; set; }
        public List<ChiNhanhToReturnViewModel> ChiNhanhToReturnViewModels { get; set; }
        public List<KhoiViewModel> KhoiViewModels { get; set; }
        public KhachHuyViewModel()
        {
            ChiNhanhToReturnViewModels = new List<ChiNhanhToReturnViewModel>();
            KhoiViewModels = new List<KhoiViewModel>();
        }
    }
}
