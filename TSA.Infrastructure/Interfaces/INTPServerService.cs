using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using TSA.Infrastructure.DTOs;
using TSA.Infrastructure.Entities;
using TSALibrary.Models;

namespace TSA.Infrastructure.Interfaces
{
    public interface INTPServerService
    {
        public Task<IEnumerable<NTPServerDTO>> GetAllNTPServers();
        public Task<NTPServerDTO> GetNTPServerById(int id);
        public Task AddAndSave(HttpContext httpContext, NTPServerDTO nTPServer);
        public Task UpdateAndSave(HttpContext httpContext, NTPServerDTO nTPServer);
        public Task SoftDeleteAndSave(HttpContext httpContext, int id);
        public bool NTPServerExists(NTPServerDTO nTPServer);

    }
}
