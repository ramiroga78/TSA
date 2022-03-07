using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TSA.Infrastructure.Data;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;

namespace TSA.Infrastructure.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        private readonly TSADbContext _context;
        public RoleRepository(TSADbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Role> GetRoleById(int id)
        {
            var role = await _entities.FindAsync(id);
            return role;
        }

        public IEnumerable<SelectListItem> GetRolesList()
        {
            return _context.Roles
                .Where(x => x.IsActive == true)
                .Select(i => new SelectListItem()
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
        }
    }
}
