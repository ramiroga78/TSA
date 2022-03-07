using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using TSA.Infrastructure.Entities;
using TSALibrary.Models;

namespace TSA.Infrastructure.Interfaces
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        public Task<Role> GetRoleById(int id);

        public IEnumerable<SelectListItem> GetRolesList();
    }
}
