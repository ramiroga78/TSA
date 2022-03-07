using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSA.Infrastructure.Data;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;

namespace TSA.Infrastructure.Repositories
{
    public class DeltaUserRepository : GenericRepository<DeltaUser>, IDeltaUserRepository
    {
        private readonly TSADbContext _context;
        public DeltaUserRepository(TSADbContext context) : base(context)
        {
            _context = context;
        }

        public List<DeltaUser> GetUsersByDeltaId(int id)
        {
            var deltaUsers = _context.DeltaUsers.Where(x => x.DeltaId == id).ToList();
            
            return deltaUsers;
        }
        public List<User> GetAllUserThatNotHaveDeltaById(int deltaId)
        {
            var users = _context.Users.ToList();
            var deltaUsers = _context.DeltaUsers.Where(x => x.DeltaId == deltaId).ToList();

            var usersThatNoHaveDeltas = from user in users
                                        where !(from deltaUser in deltaUsers select deltaUser.UserId)
                                        .Contains(user.Id)
                                            || (from deltaUser in deltaUsers where deltaUser.IsActive == false select deltaUser.UserId)
                                        .Contains(user.Id)
                                        select user;
            
            return usersThatNoHaveDeltas.ToList();
        }
        public DeltaUser GetDeltaUserByDeltaIdAndDeltaId(int deltaId, int userId)
        {
            var deltaUser = _context.DeltaUsers.Where(x => x.DeltaId == deltaId && x.UserId == userId).FirstOrDefault();

            return deltaUser;
        }
    }
}
