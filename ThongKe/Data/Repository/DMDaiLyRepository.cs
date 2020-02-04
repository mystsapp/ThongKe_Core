using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThongKe.Data.Interfaces;
using ThongKe.Data.Models;

namespace ThongKe.Data.Repository
{
    public interface IDMDaiLyRepository : IRepository<Dmdaily> { }
    public class DMDaiLyRepository : Repository<Dmdaily>, IDMDaiLyRepository
    {
        public DMDaiLyRepository(thongkeContext context) : base(context)
        {

        }
    }
}
