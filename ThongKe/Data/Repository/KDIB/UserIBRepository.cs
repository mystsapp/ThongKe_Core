using System;
using System.Collections.Generic;
using System.Linq;
using ThongKe.Data.Models_KDIB;

namespace ThongKe.Data.Repository.KDIB
{
    public interface IUserIBRepository
    {
        IEnumerable<Users> Find(Func<Users, bool> value);
        IEnumerable<Users> GetUsers();
    }
    public class UserIBRepository : IUserIBRepository
    {
        private readonly SaleDoanIBContext _context;

        public UserIBRepository(SaleDoanIBContext context)
        {
            _context = context;
        }

        public IEnumerable<Users> Find(Func<Users, bool> value)
        {
            return _context.Users.Where(value);
        }

        public IEnumerable<Users> GetUsers()
        {
            return _context.Users;
        }
    }
}
