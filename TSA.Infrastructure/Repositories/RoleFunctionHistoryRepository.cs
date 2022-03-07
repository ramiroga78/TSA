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
    public class RoleFunctionHistoryRepository : GenericRepository<RoleFunctionHistory>, IRoleFunctionHistoryRepository
    {
        TSADbContext _context = new TSADbContext();
        public RoleFunctionHistoryRepository(TSADbContext context) : base(context)
        {
        }
    }
}
