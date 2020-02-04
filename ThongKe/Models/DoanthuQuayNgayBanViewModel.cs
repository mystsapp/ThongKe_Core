using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThongKe.Data.Models;

namespace ThongKe.Models
{
    public class DoanthuQuayNgayBanViewModel
    {
        public IEnumerable<DoanthuQuayNgayBan> DoanthuQuayNgayBans { get; set; }
        public List<ChiNhanhToReturnViewModel> chiNhanhToReturnViewModels { get; set; }
        public DoanthuQuayNgayBanViewModel()
        {
            chiNhanhToReturnViewModels = new List<ChiNhanhToReturnViewModel>();
        }
    }
}
