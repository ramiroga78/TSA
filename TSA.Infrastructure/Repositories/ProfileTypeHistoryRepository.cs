using System.Threading.Tasks;
using TSA.Infrastructure.Data;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;


namespace TSA.Infrastructure.Repositories
{
   public class ProfileTypeHistoryRepository : GenericRepository<ProfileTypesHistory>, IProfileTypeHistoryRepository
   {
        public ProfileTypeHistoryRepository(TSADbContext context) : base(context)
        {
        }
   }
}
