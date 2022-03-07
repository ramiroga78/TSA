using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TSA.Infrastructure.Data;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;

namespace TSA.Infrastructure.Repositories
{
    public class FunctionRepository : GenericRepository<Function>, IFunctionRepository
    {
        public FunctionRepository(TSADbContext context) : base(context)
        {
        }

        TSADbContext _context = new TSADbContext();
        public List<Function> GetAllFunctions()
        {
            var functions = _context.Functions.ToList();
            return functions;
        }
    }
}
