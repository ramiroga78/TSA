using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using TSA.Infrastructure.Data;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;
using System.Linq;
using System.Collections.Generic;

namespace TSA.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(TSADbContext context) : base(context)
        {
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _entities.Include(u => u.RoleUsers).FirstOrDefaultAsync(x => x.Email.ToLower().Equals(email.ToLower()));
        }

        public Task<User> GetByIdAsyncIncludingRoles(int id)
        {
            return _entities.Include(u => u.RoleUsers).Where(r => r.Id == id).FirstOrDefaultAsync();
        }

        public async Task SoftDelete(int id)
        {
            var user = await _entities.FindAsync(id);
            user.IsActive = false;
        }

        public async Task<bool> UserExists(User user)
        {
            return await _entities.AnyAsync(x => x.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase));
        }

    }
}
