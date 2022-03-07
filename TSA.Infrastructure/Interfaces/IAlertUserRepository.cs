using System.Collections.Generic;
using System.Threading.Tasks;
using TSA.Infrastructure.DTOs;
using TSA.Infrastructure.Entities;
using TSALibrary.Models;

namespace TSA.Infrastructure.Interfaces
{
    public interface IAlertUserRepository : IGenericRepository<AlertUser>
    {
        public List<AlertUser> GetUsersByAlertId(int id);
        public List<User> GetAllUserThatNotHaveAlertById(int alertId);
        public AlertUser GetAlertUserByAlertIdAndUserId(int alertId, int userId);
    }
}
