using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using TSA.Infrastructure.Entities;
using TSALibrary.Models;

namespace TSA.Infrastructure.Interfaces
{
    public interface IDeltaRepository : IGenericRepository<Delta>
    {
        public Task<Delta> GetDeltaById(int id);
    }
}
