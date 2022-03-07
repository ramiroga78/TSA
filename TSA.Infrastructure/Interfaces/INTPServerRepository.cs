using System.Threading.Tasks;
using TSA.Infrastructure.Entities;
using TSALibrary.Models;

namespace TSA.Infrastructure.Interfaces
{
    public interface INTPServerRepository : IGenericRepository<NTPServer>
    {
        public Task GetAllNTPServers();
        public Task SoftDelete(int id);

    }
}
