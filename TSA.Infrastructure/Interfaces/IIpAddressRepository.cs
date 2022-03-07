using System.Threading.Tasks;
using TSA.Infrastructure.Entities;
using TSALibrary.Models;

namespace TSA.Infrastructure.Interfaces
{
   public interface IIpAddressRepository : IGenericRepository<IpAddress>
    {
        public Task<IpAddress> GetIpAddressById(int Id);
        public Task<bool> Exists(string ip);
    }
}
