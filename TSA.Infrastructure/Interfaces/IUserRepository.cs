using System.Threading.Tasks;
using TSA.Infrastructure.Entities;
using TSALibrary.Models;

namespace TSA.Infrastructure.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        public Task SoftDelete(int id);
        public Task<bool> UserExists(User user);
        public Task<User> GetUserByEmail(string email);
        public Task<User> GetByIdAsyncIncludingRoles(int id);
    }
}
