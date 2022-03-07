using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TSA.Infrastructure.DTOs;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;

namespace TSA.Infrastructure.Services
{
    public class IpAddressService : BaseService, IIpAddresService
    {
        public IpAddressService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public IpAddressDTO CreateIpAddressDTO()
        {
            try
            {
                IpAddressDTO ipAddressDTO = new IpAddressDTO();

                return ipAddressDTO;
            }
            catch (System.Exception)
            {
                throw;
            }

        }
        public async Task<IEnumerable<IpAddressDTO>> GetAllIpAddresses()
        {
            try
            {
                var ipAddress = await _unitOfWork.IpAddressRepository.GetAllAsync(x => x.IsActive == true);
                var ipAddressDTO = _mapper.Map<IEnumerable<IpAddressDTO>>(ipAddress);

                return ipAddressDTO;
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        public async Task<IpAddressDTO> GetAllIpAddressByIdAndModelToDto(int id)
        {
            try
            {
                var ipAddress = await _unitOfWork.IpAddressRepository.GetIpAddressById(id);
                var ipAddressDTO = _mapper.Map<IpAddressDTO>(ipAddress);

                return ipAddressDTO;
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        public async Task AddAndSave(HttpContext httpContext, IpAddressDTO ipddressDTO)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var ipAddress = _mapper.Map<IpAddress>(ipddressDTO);

                //Get current logged-in userId
                ipAddress.AddUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                ipAddress.AddDate = DateTime.Now;
                ipAddress.IsActive = true;

                await _unitOfWork.IpAddressRepository.Insert(ipAddress);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (System.Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }
        public async Task UpdateAndSave(HttpContext httpContext, IpAddressDTO ipAddressDTO)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var ipAddress = await _unitOfWork.IpAddressRepository.GetIpAddressById(ipAddressDTO.Id);
                //IpAddress
                ipAddress.Name = ipAddressDTO.Name;
                ipAddress.Ip = ipAddressDTO.Ip;
                //Get current logged-in userId
                ipAddress.EditUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                ipAddress.EditDate = DateTime.Now;

                _unitOfWork.IpAddressRepository.Update(ipAddress);

                //IpAddressHostory
                IpAddressesHistory ipAddressesHistory = new IpAddressesHistory();
                ipAddressesHistory.IdHistory = ipAddress.Id;
                ipAddressesHistory.Name = ipAddress.Name;
                ipAddressesHistory.AddUserId = ipAddress.AddUserId;
                ipAddressesHistory.EditUserId = ipAddress.EditUserId;
                ipAddressesHistory.DeleteUserId = ipAddress.DeleteUserId;
                ipAddressesHistory.AddDate = ipAddress.AddDate;
                ipAddressesHistory.EditUserId = ipAddress.EditUserId;
                ipAddressesHistory.DeleteDate = ipAddress.DeleteDate;
                ipAddressesHistory.IsActive = ipAddress.IsActive;

                await _unitOfWork.IpAddressHistoryRepository.Insert(ipAddressesHistory);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (System.Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }
        public async Task SoftDeleteAndSave(HttpContext httpContext, int id)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var ipAddress = await _unitOfWork.IpAddressRepository.GetIpAddressById(id);
                //IpAddress
                //Get current logged-in userId
                ipAddress.DeleteUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                ipAddress.DeleteDate = DateTime.Now;
                ipAddress.IsActive = false;

                _unitOfWork.IpAddressRepository.Update(ipAddress);

                //IpAddressHOstory
                IpAddressesHistory ipAddressesHistory = new IpAddressesHistory();
                ipAddressesHistory.IdHistory = ipAddress.Id;
                ipAddressesHistory.Name = ipAddress.Name;
                ipAddressesHistory.AddUserId = ipAddress.AddUserId;
                ipAddressesHistory.EditUserId = ipAddress.EditUserId;
                ipAddressesHistory.DeleteUserId = ipAddress.DeleteUserId;
                ipAddressesHistory.AddDate = ipAddress.AddDate;
                ipAddressesHistory.EditUserId = ipAddress.EditUserId;
                ipAddressesHistory.DeleteDate = ipAddress.DeleteDate;
                ipAddressesHistory.IsActive = ipAddress.IsActive;

                await _unitOfWork.IpAddressHistoryRepository.Insert(ipAddressesHistory);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (System.Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        public Task<bool> Exists(string ip)
        {
            return  _unitOfWork.IpAddressRepository.Exists(ip);
        }
    }
}