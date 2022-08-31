using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThongKe.Data.Models;

namespace ThongKe.Models
{
    public class TuyentqTheoNgayBanViewModel
    {
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
        public string Khoi { get; set; }

        public IEnumerable<TuyenTqTheoNgayBan> TuyenTqTheoNgayBans { get; set; }
        public List<ChiNhanhToReturnViewModel> ChiNhanhToReturnViewModels { get; set; }
        public List<KhoiViewModel> KhoiViewModels_KL { get; set; }
        public TuyentqTheoNgayBanViewModel()
        {
            ChiNhanhToReturnViewModels = new List<ChiNhanhToReturnViewModel>();
            KhoiViewModels_KL = new List<KhoiViewModel>();
        }
    }
}
