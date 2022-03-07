using System.Threading.Tasks;
using TSA.Infrastructure.Entities;
using TSALibrary.Models;

namespace TSA.Infrastructure.Interfaces
{
    public interface IExternalUserRepository : IGenericRepository<ExternalUser>
    {
        public Task<ExternalUser> GetUserByEmail(string email);
        public Task<ExternalUser> GetByIdAsyncIncludingRoles(int id);
        public Task SoftDelete(int id);
        public Task<bool> UserExists(ExternalUser user);


    }
}
