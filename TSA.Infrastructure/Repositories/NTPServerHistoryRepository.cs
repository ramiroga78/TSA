using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TSA.Infrastructure.Data;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;

namespace TSA.Infrastructure.Repositories
{
    public class NTPServerHistoryRepository : GenericRepository<NTPServerHistory>, INTPServerHistoryRepository
    {
        public NTPServerHistoryRepository(TSADbContext context) : base(context)
        {
        }
    }
}
