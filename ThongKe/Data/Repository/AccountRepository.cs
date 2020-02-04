using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThongKe.Data.Interfaces;
using ThongKe.Data.Models;
using ThongKe.Models;

namespace ThongKe.Data.Repository
{
    public interface IAccountRepository : IRepository<Account>
    {
        IEnumerable<TuyenThamQuanViewModel> GetAllTuyentqByKhoi(string khoi);
    }
    public class AccountRepository : Repository<Account>, IAccountRepository
    {
        public AccountRepository(thongkeContext context) : base(context)
        {
        }

        public IEnumerable<TuyenThamQuanViewModel> GetAllTuyentqByKhoi(string khoi)
        {
            var parameter = new SqlParameter[]
            {
                new SqlParameter("@khoi",khoi)
            };
            //var result = _context.Database.SqlQuery<string>("spLoadTuyentq @khoi", parammeter);
            var result = _context.Tuyentq.FromSqlRaw("EXECUTE dbo.spLoadTuyentq @khoi", parameter).ToList();
            return result;
        }

    }
}
