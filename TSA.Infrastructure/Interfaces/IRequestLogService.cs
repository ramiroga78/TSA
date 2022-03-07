using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using TSA.Infrastructure.DTOs;
using TSA.Infrastructure.Entities;
using TSALibrary.Models;

namespace TSA.Infrastructure.Interfaces
{
    public interface IRequestLogService
    {
        public Task<IEnumerable<RequestLog>> GetAllRequestLog();
        public Task<RequestLog> GetRequestLogById(int id);
        public Task UpdateAndSave(RequestLog Request);
        public Task<int> AddAndSave(RequestLog Request);
        public IQueryable<RequestLog> GetIQueryable();
        public Task<int> CountRequestLog();
        public Task<string> GetRequest(int id);
        public Task<string> GetResponse(int id);
    }
}
