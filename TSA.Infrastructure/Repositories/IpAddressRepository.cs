using System.Threading.Tasks;
using TSA.Infrastructure.Data;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;
using System.Linq;

namespace TSA.Infrastructure.Repositories
{
    public class IpAddressRepository : GenericRepository<IpAddress>, IIpAddressRepository
    {
        public IpAddressRepository(TSADbContext context) : base(context)
        {
        }

        public async Task<IpAddress> GetIpAddressById(int id)
        {
            var IpAddress = await _entities.FindAsync(id);
            return IpAddress;
        }
        public virtual async Task<bool> Exists(string ip)
        {
            return  _entities.Any<IpAddress>(x=>x.Ip.Equals(ip));
        }

    }
}

