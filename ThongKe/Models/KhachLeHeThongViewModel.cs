﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThongKe.Data.Models;

namespace ThongKe.Models
{
    public class KhachLeHeThongViewModel
    {
        public IEnumerable<DoanhthuToanhethong> DoanhthuToanhethongs { get; set; }
        public List<ChiNhanhToReturnViewModel> ChiNhanhToReturnViewModels { get; set; }
        public List<KhoiViewModel> KhoiViewModels { get; set; }
        public KhachLeHeThongViewModel()
        {
            ChiNhanhToReturnViewModels = new List<ChiNhanhToReturnViewModel>();
            KhoiViewModels = new List<KhoiViewModel>();
        }

    }
}