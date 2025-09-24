using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models_Tourleob
{
    public partial class QuydinhtourLog
    {
        public decimal Id { get; set; }
        public string Sgtcode { get; set; }
        public string Quydinh { get; set; }
        public string Logfile { get; set; }
        public DateTime? Ngaycapnhat { get; set; }
    }
}
