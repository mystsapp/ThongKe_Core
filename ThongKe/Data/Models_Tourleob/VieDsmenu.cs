using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models_Tourleob
{
    public partial class VieDsmenu
    {
        public int Menuid { get; set; }
        public string Menunm { get; set; }
        public string Menulink { get; set; }
        public int? Areaid { get; set; }
        public bool ShowMk { get; set; }
        public string Classcss { get; set; }
        public string Role { get; set; }
        public string Actionnm { get; set; }
        public string Controllernm { get; set; }
        public string Areamvc { get; set; }
        public int? Thutu { get; set; }
        public string Areaname { get; set; }
        public string Areacss { get; set; }
        public int? Tt { get; set; }
    }
}
