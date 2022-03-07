using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TSA.Infrastructure.Data;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace TSA.Infrastructure.Repositories
{
    public class RoleFunctionRepository : GenericRepository<RoleFunction>, IRoleFunctionRepository
    {
        TSADbContext _context = new TSADbContext();
        public RoleFunctionRepository(TSADbContext context) : base(context)
        {
        }

        public List<RoleFunction> GetRoleFunctionsByRoleId (int roleId)
        {
            var roleFunctions = _context.RolesFunctions.Where(x => x.RoleId == roleId).ToList();
            return roleFunctions;
        }

        public RoleFunction GetRoleFunctionById(int roleId, int functionId)
        {
            var roleFunction = _context.RolesFunctions.Where(x => x.RoleId == roleId && x.FunctionId == functionId).FirstOrDefault();
            return roleFunction;     
        }
    }
}
