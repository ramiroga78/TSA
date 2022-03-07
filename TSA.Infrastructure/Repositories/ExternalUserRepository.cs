using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TSA.Infrastructure.Data;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;

namespace TSA.Infrastructure.Repositories
{
    public class ExternalUserRepository : GenericRepository<ExternalUser>, IExternalUserRepository
    {
        public ExternalUserRepository(TSADbContext context) : base(context)
        {
        }

        public async Task<ExternalUser> GetUserByEmail(string email)
        {
            return await _entities.FirstOrDefaultAsync(x => x.Email.ToLower().Equals(email.ToLower()) && x.IsActive==true);
        }

        public Task<ExternalUser> GetByIdAsyncIncludingRoles(int id)
        {
            return _entities.Where(r => r.Id == id).FirstOrDefaultAsync();
        }

        public async Task SoftDelete(int id)
        {
            var user = await _entities.FindAsync(id);
            user.IsActive = false;
        }

        public async Task<bool> UserExists(ExternalUser user)
        {
            return await _entities.AnyAsync(x => x.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase));
        }

    }
}
