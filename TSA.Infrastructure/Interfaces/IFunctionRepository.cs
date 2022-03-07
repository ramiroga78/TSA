using System.Collections.Generic;
using System.Threading.Tasks;
using TSA.Infrastructure.Entities;
using TSALibrary.Models;

namespace TSA.Infrastructure.Interfaces
{
    public interface IFunctionRepository : IGenericRepository<Function>
    {
        public List<Function> GetAllFunctions();
    }
}
