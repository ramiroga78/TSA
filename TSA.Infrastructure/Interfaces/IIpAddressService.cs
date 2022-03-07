using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using TSA.Infrastructure.DTOs;

namespace TSA.Infrastructure.Interfaces
{
    public interface IIpAddresService
    {
        public Task<IEnumerable<IpAddressDTO>> GetAllIpAddresses();
        public Task<IpAddressDTO> GetAllIpAddressByIdAndModelToDto(int id);
        public Task UpdateAndSave(HttpContext httpContext, IpAddressDTO role);
        public Task SoftDeleteAndSave(HttpContext httpContext, int id);
        //public Task<bool> IpAddressExist(IpAddressDTO role);
        public Task AddAndSave(HttpContext httpContext, IpAddressDTO role);
        public IpAddressDTO CreateIpAddressDTO();
        public Task<bool> Exists(string ip);

    }
}
