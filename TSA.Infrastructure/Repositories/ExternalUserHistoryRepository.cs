using TSA.Infrastructure.Data;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;

namespace TSA.Infrastructure.Repositories
{
    public class ExternalUserHistoryRepository : GenericRepository<ExternalUserHistory>, IExternalUserHistoryRepository
    {
        public ExternalUserHistoryRepository(TSADbContext context) : base(context)
        {
        }
    }
}
