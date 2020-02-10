using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ThongKe.Models
{
    public class KhachHuy
    {
        [Key]
        public long stt { get; set; }
        public string tenkhach { get; set; }
        public string sgtcode { get; set; }
        public int vetourid { get; set; }
        public string tuyentq { get; set; }
        public DateTime batdau { get; set; }
        public DateTime ketthuc { get; set; }
        public decimal giatour { get; set; }
        public string nguoihuyve { get; set; }
        public string dailyhuyve { get; set; }
        public string chinhanh { get; set; }
        public DateTime ngayhuyve { get; set; }
    }
}
