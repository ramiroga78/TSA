using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TSA.Infrastructure.Data;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;
namespace TSA.Infrastructure.Repositories
{
    public class DeltaTypeRepository : GenericRepository<DeltaType>, IDeltaTypeRepository
    {
        TSADbContext _context = new TSADbContext();
        public DeltaTypeRepository(TSADbContext context) : base(context)
        {
        }

        public List<DeltaType> GetDeltaTypeByDeltaId (int deltaId)
        {
            var deltaTypes = _context.DeltaTypes.Where(x => x.Id == deltaId).ToList();
            return deltaTypes;
        }

        public DeltaType GetDeltaTypeById(int deltaId, int typeId)
        {
            var deltaTypes = _context.DeltaTypes.Where(x => x.Id == deltaId && x.Id == typeId).FirstOrDefault();
            return deltaTypes;
        }
    }
}
