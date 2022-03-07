using System.Collections.Generic;
using System.Threading.Tasks;
using TSA.Infrastructure.Entities;
using TSALibrary.Models;


namespace TSA.Infrastructure.Interfaces
{
   public interface IDeltaUserRepository : IGenericRepository<DeltaUser>
    {
        public List<DeltaUser> GetUsersByDeltaId(int id);
        public List<User> GetAllUserThatNotHaveDeltaById(int alertId);
        public DeltaUser GetDeltaUserByDeltaIdAndDeltaId(int alertId, int userId);
    }
}
