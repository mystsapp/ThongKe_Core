using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ThongKe.Models
{
    public class TourBySGTCodeViewModel
    {
        [Key]
        public string sgtcode { get; set; }
        public string tuyentq { get; set; }
        public DateTime batdau { get; set; }
        public DateTime ketthuc { get; set; }
    }
}
