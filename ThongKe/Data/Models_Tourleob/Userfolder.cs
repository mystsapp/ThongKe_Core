using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models_Tourleob
{
    public partial class Userfolder
    {
        public string UserId { get; set; }
        public string Phongban { get; set; }
        public bool? Show { get; set; }
        public bool? Upload { get; set; }
    }
}
