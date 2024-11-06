using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models
{
    public partial class LoginViewModels
    {
        public string Username { get; set; }
        public string Mact { get; set; }
        public string Password { get; set; }
        public bool Trangthai { get; set; }
        public bool Doimk { get; set; }
    }
}
