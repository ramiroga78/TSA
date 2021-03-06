using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSALibrary.Models;

namespace TSA.Infrastructure.Interfaces
{
    public interface IIpAddressHistoryRepository :  IGenericRepository<IpAddressesHistory>
    {
        public void SaveChangesIpAddressHistoryRepository(IpAddressesHistory ipAddressHistory);
    }
}
