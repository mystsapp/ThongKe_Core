using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThongKe.Data.Models;

namespace ThongKe.Models
{
    public class DoanhThuSaleTuyenViewModel
    {
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
        public string Khoi { get; set; }

        public IEnumerable<DoanhthuSaleTuyen> DoanhthuSaleTuyens { get; set; }
        public IEnumerable<TuyenThamQuanViewModel> tuyenThamQuanViewModels { get; set; }
        public List<KhoiViewModel> KhoiViewModels_KL { get; set; }
        public DoanhThuSaleTuyenViewModel()
        {
            KhoiViewModels_KL = new List<KhoiViewModel>();
        }
    }
}
