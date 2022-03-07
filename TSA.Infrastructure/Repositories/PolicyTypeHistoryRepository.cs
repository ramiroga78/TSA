using System.Threading.Tasks;
using TSA.Infrastructure.Data;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;

namespace TSA.Infrastructure.Repositories
{
    public class PolicyTypeHistoryRepository : GenericRepository<PoliciesTypesHistory>, IPolicyTypeHistoryRepository
    {
        public PolicyTypeHistoryRepository(TSADbContext context) : base(context)
        {
        }
    }
}
