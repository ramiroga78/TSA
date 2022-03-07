using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSALibrary.Models;

namespace TSA.Infrastructure.Interfaces
{
   public interface IDeltaTypeRepository : IGenericRepository<DeltaType>
    {
        public List<DeltaType> GetDeltaTypeByDeltaId(int deltaId);
        public DeltaType GetDeltaTypeById(int deltaId, int deltaTypeId);
    }
}
