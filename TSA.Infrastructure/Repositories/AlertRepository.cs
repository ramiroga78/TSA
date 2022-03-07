using System.Linq;
using System.Threading.Tasks;
using TSA.Infrastructure.Data;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;

namespace TSA.Infrastructure.Repositories
{
    public class AlertRepository : GenericRepository<Alert>, IAlertRepository
    {
        public AlertRepository(TSADbContext context) : base(context)
        {
        }
        private readonly TSADbContext _context;
        public async Task<Alert> GetAlertById(int id)
        {
            var alert = await _entities.FindAsync(id);
            return alert;
        }
    }
}
