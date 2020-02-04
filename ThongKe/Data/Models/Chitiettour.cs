using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models
{
    public partial class Chitiettour
    {
        public string Sgtcode { get; set; }
        public string Tuyentq { get; set; }
        public DateTime? Batdau { get; set; }
        public DateTime? Ketthuc { get; set; }
    }
}
