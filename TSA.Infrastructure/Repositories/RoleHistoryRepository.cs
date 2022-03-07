using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSA.Infrastructure.Data;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;

namespace TSA.Infrastructure.Repositories
{
    public class RoleHistoryRepository : GenericRepository<RolesHistory>, IRoleHistoryRepository
    {
        public RoleHistoryRepository(TSADbContext context) : base(context)
        {
        }

        public void SaveChangesRoleHistory(RolesHistory roleHistory)
        {
             //_entities.Add(roleHistory);
        }
    }
}
