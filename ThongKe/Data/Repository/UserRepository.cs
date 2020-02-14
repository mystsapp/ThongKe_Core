using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using ThongKe.Data.Interfaces;
using ThongKe.Data.Models;
using ThongKe.Helps;
using ThongKe.Models;

namespace ThongKe.Data.Repository
{
    public interface IUserRepository : IRepository<Users>
    {
        IEnumerable<TuyenThamQuanViewModel> GetAllTuyentqByKhoi(string khoi);

        LoginViewModel Login(string username, string mact);

        int Changepass(string username, string newpass);
    }

    public class UserRepository : Repository<Users>, IUserRepository
    {
        public UserRepository(thongkeContext context) : base(context)
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

        public LoginViewModel Login(string username, string mact)
        {
            var parammeter = new SqlParameter[]
           {
                new SqlParameter("@username",username),
                new SqlParameter("@mact",mact)
           };

            var result = _context.LoginViewModels.FromSqlRaw("dbo.spLogin @username, @mact", parammeter).ToList();
            if (result == null)
            {
                return null;
            }
            else
            {
                return result.SingleOrDefault();
            }
        }

        public int Changepass(string username, string newpass)
        {
            try
            {
                var result = GetById(username);

                result.Password = newpass;
                result.Doimatkhau = false;
                result.Ngaydoimk = DateTime.Now;
                _context.SaveChanges();
                return 1;
            }
            catch { throw; }
        }
    }
}