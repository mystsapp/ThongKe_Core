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
        public List<KhoiViewModel> KhoiViewModels_KL { get; set; }
        public KhachHuyViewModel()
        {
            ChiNhanhToReturnViewModels = new List<ChiNhanhToReturnViewModel>();
            KhoiViewModels_KL = new List<KhoiViewModel>();
        }
    }
}
