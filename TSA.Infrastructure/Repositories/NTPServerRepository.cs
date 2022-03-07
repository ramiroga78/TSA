using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TSA.Infrastructure.Data;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;

namespace TSA.Infrastructure.Repositories
{
    public class NTPServerRepository : GenericRepository<NTPServer>, INTPServerRepository
    {
        private readonly TSADbContext _context;

        public NTPServerRepository(TSADbContext context) : base(context)
        {
            _context = context;
        }

        public Task GetAllNTPServers()
        {
            throw new System.NotImplementedException();
        }

        public async Task SoftDelete(int id)
        {
            //throw new System.NotImplementedException();
            var nTPServer = await _entities.FindAsync(id);
            nTPServer.IsActive = false;
        }
    }
}
