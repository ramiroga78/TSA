using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSA.Infrastructure.Entities;
using TSALibrary.Models;

namespace TSA.Infrastructure.Interfaces
{
    public interface IRoleHistoryRepository : IGenericRepository<RolesHistory>
    {
        public void SaveChangesRoleHistory(RolesHistory roleHistory);
    }
}
