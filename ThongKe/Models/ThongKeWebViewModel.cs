using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThongKe.Data.Models;

namespace ThongKe.Models
{
    public class ThongKeWebViewModel
    {
        public IEnumerable<Thongkeweb> Thongkewebs { get; set; }
        public List<KhoiViewModel> KhoiViewModels_KL { get; set; }
        public ThongKeWebViewModel()
        {
            KhoiViewModels_KL = new List<KhoiViewModel>();
        }
    }
}
