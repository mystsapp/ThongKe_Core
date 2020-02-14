using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ThongKe.Data.Models;

namespace ThongKe.Models
{
    public class UserViewModel
    {
        public IEnumerable<Users> Users { get; set; }
        public Users User { get; set; }
        public IEnumerable<Chinhanh> Chinhanhs { get; set; }
        public IEnumerable<Dmdaily> Dmdailies { get; set; }
        public IEnumerable<KhoiViewModel> KhoiViewModels { get; set; }
        public IEnumerable<RoleViewModel> RoleViewModels { get; set; }
        public string OldPass { get; set; }
        [DataType(DataType.Password)]
        public string PassToEdit { get; set; }
    }
}
