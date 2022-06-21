using System;
using System.Collections.Generic;

namespace ThongKe.Data.Models_KDIB
{
    public partial class Roles
    {
        public Roles()
        {
            Users = new HashSet<Users>();
        }

        public int Id { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }

        public virtual PhanKhuCns PhanKhuCns { get; set; }
        public virtual ICollection<Users> Users { get; set; }
    }
}
