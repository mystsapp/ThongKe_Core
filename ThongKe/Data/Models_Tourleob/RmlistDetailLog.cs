using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models_Tourleob
{
    public partial class RmlistDetailLog
    {
        public decimal Id { get; set; }
        public string Sgtcode { get; set; }
        public string Supplierid { get; set; }
        public string Serial { get; set; }
        public string Sttkhach { get; set; }
        public string Tenkhach { get; set; }
        public DateTime? Ngaycapnhat { get; set; }
        public string Nguoicapnhat { get; set; }
        public string Loai { get; set; }
    }
}
