using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThongKe.Data.Interfaces;
using ThongKe.Data.Models;

namespace ThongKe.Data.Repository
{
    public interface IChiNhanhRepository : IRepository<Chinhanh>
    {

    }
    public class ChiNhanhRepository : Repository<Chinhanh>, IChiNhanhRepository
    {
        public ChiNhanhRepository(thongkeContext context) : base(context)
        {

        }
    }
}
