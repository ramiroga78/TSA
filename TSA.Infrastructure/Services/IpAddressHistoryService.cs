using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;

namespace TSA.Infrastructure.Services
{
    public class IpAddressHistoryService : BaseService, IIpAddressHistoryService
    {
        public IpAddressHistoryService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public async Task SetChanges(IpAddress ipAddress)
        {
            try
            {
                IpAddressesHistory ipAddressesHistory = new IpAddressesHistory();

                ipAddressesHistory.IdHistory = ipAddress.Id;
                ipAddressesHistory.Name = ipAddress.Name;
                ipAddressesHistory.Ip = ipAddress.Ip;
                ipAddressesHistory.AddUserId = ipAddress.AddUserId;
                ipAddressesHistory.EditUserId = ipAddress.EditUserId;
                ipAddressesHistory.DeleteUserId = ipAddress.DeleteUserId;
                ipAddressesHistory.IsActive = (bool)ipAddress.IsActive;

                await _unitOfWork.IpAddressHistoryRepository.Insert(ipAddressesHistory);
                await _unitOfWork.SaveChangesAsync();

            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}
