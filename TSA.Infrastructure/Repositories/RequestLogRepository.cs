using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TSA.Infrastructure.Data;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;

namespace TSA.Infrastructure.Repositories
{
    public class RequestLogRepository : GenericRepository<RequestLog>, IRequestLogRepository
    {
        private readonly TSADbContext _context;
        public RequestLogRepository(TSADbContext context) : base(context)
        {
            _context = context;
        }
        public IQueryable<RequestLog> GetIQueryable()
        {
            return _context.RequestLogs.AsQueryable();
        }
        public async Task<int> CountRequestLog()
        {
           return await _context.RequestLogs.CountAsync();
        }

    }
}
