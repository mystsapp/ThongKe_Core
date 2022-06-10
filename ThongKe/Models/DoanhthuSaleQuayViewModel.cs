using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThongKe.Data.Models;

namespace ThongKe.Models
{
    public class DoanhthuSaleQuayViewModel
    {
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
        public string Khoi { get; set; }
        public IEnumerable<DoanhthuSaleQuay> DoanhthuSaleQuays { get; set; }
        public List<ChiNhanhToReturnViewModel> chiNhanhToReturnViewModels { get; set; }
        public List<KhoiViewModel> KhoiViewModels_KL { get; set; }
        public DoanhthuSaleQuayViewModel()
        {
            chiNhanhToReturnViewModels = new List<ChiNhanhToReturnViewModel>();
            KhoiViewModels_KL = new List<KhoiViewModel>();
        }
    }
}
