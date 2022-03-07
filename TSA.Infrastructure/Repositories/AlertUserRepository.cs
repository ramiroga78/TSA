using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TSA.Infrastructure.Data;
using TSA.Infrastructure.Interfaces;
using TSA.Infrastructure.Repositories;
using TSALibrary.Models;

namespace TSA.Infrastructure.Repositories
{
    public class AlertUserRepository : GenericRepository<AlertUser>, IAlertUserRepository
    {
        private readonly TSADbContext _context;
        public AlertUserRepository(TSADbContext context) : base(context)
        {
            _context = context;
        }
        public List<AlertUser> GetUsersByAlertId(int id)
        {
            var alertUsers = _context.AlertUsers.Where(x => x.AlertId == id).ToList();
            return alertUsers;
        }
        public List<User> GetAllUserThatNotHaveAlertById(int alertId)
        {
            var users = _context.Users.ToList();
            var alertUsers = _context.AlertUsers.Where(x => x.AlertId == alertId).ToList();
            //ACÁ TRAIGO LOS USUARIOS QUE NO TIENEN ALERTA O QUE SI TIENEN PERO SU ESTADO ES INACTIVO
            var usersThatNoHaveAlerts = from user in users
                                        where !(from alertUser in alertUsers select alertUser.UserId)
                                        .Contains(user.Id)
                                              || (from alertUser in alertUsers where alertUser.IsActive == false select alertUser.UserId)
                                        .Contains(user.Id)
                                        select user;
            return usersThatNoHaveAlerts.ToList();
        }

        public AlertUser GetAlertUserByAlertIdAndUserId(int alertId, int userId)
        {
            var alertUser = _context.AlertUsers.Where(x => x.AlertId == alertId && x.UserId == userId).FirstOrDefault();
            return alertUser;
        }
    }
}
