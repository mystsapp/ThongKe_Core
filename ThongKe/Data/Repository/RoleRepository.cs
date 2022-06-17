using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThongKe.Data.Models;

namespace ThongKe.Data.Repository
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetRoles();
        Task<Role> GetRoleById(int id);
    }
    public class RoleRepository : IRoleRepository
    {
        private readonly thongkeContext _context;

        public RoleRepository(thongkeContext context)
        {
            _context = context;
        }

        public async Task<Role> GetRoleById(int id)
        {
            return await _context.Roles.FindAsync(id);
        }

        public async Task<IEnumerable<Role>> GetRoles()
        {
            return await _context.Roles.ToListAsync();
        }
    }
}
