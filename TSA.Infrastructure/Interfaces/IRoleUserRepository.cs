using System.Threading.Tasks;
using TSA.Infrastructure.Entities;
using TSALibrary.Models;

namespace TSA.Infrastructure.Interfaces
{
    public interface IRoleUserRepository : IGenericRepository<RoleUser>
    {
        public Task<int> GetUserRoleByUserId(int id);
    }
}
