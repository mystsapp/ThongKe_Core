using System;
using System.Collections.Generic;
using System.Linq;
using ThongKe.Data.Models_KDIB;

namespace ThongKe.Data.Repository.KDIB
{
    public interface IUserIBRepository
    {
        IEnumerable<Users> Find(Func<Users, bool> value);
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
    }
}
