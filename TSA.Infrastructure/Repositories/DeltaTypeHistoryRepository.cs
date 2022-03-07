using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TSA.Infrastructure.Data;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;


namespace TSA.Infrastructure.Repositories
{
    public class DeltaTypeHistoryRepository : GenericRepository<DeltaTypeHistory>, IDeltaTypeHistoryRepository
    {
        public DeltaTypeHistoryRepository(TSADbContext context) : base(context)
        {
        }   
    }
}
