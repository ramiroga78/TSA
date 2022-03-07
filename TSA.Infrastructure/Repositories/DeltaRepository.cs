using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TSA.Infrastructure.Data;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;

namespace TSA.Infrastructure.Repositories
{
   public class DeltaRepository : GenericRepository<Delta>, IDeltaRepository
    {
        private readonly TSADbContext _context;
        public DeltaRepository(TSADbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Delta> GetDeltaById(int id)
        {
            var delta = await _entities.FindAsync(id);
            return delta;
        }
    }
}
