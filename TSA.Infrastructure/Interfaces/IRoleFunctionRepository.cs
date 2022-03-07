using System.Collections.Generic;
using System.Threading.Tasks;
using TSA.Infrastructure.Entities;
using TSALibrary.Models;

namespace TSA.Infrastructure.Interfaces
{
    public interface IRoleFunctionRepository : IGenericRepository<RoleFunction>
    {
        public List<RoleFunction> GetRoleFunctionsByRoleId(int roleId);
        public RoleFunction GetRoleFunctionById(int roleId, int functionId);
    }
}
