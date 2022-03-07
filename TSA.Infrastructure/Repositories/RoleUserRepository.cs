using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using TSA.Infrastructure.Data;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;
using System.Linq;
using System.Collections.Generic;

namespace TSA.Infrastructure.Repositories
{
    public class RoleUserRepository : GenericRepository<RoleUser>, IRoleUserRepository
    {
        public RoleUserRepository(TSADbContext context) : base(context)
        {
        }

        public async Task<int> GetUserRoleByUserId(int id)
        {
            RoleUser roleUser = await _entities.FirstOrDefaultAsync(x => x.UserId == id);

            if (roleUser != null)
                return roleUser.RoleId;
            else
                return 0;
        }
    }
}
