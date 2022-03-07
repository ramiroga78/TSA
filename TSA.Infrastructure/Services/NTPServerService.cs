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
    public class NTPServerService : BaseService, INTPServerService
    {
        private new readonly IUnitOfWork _unitOfWork;
        private new readonly IMapper _mapper;
        public NTPServerService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddAndSave(HttpContext httpContext, NTPServerDTO nTPServer)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                NTPServer server = _mapper.Map<NTPServer>(nTPServer);
                server.AddDate = DateTime.Now;
                //Get current logged-in userId
                server.AddUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                server.IsActive = true;

                await _unitOfWork.NTPServerRepository.Insert(server);

                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitAsync();
            }
            catch (System.Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<NTPServerDTO>> GetAllNTPServers()
        {
            try
            {
                var nTPServers = await _unitOfWork.NTPServerRepository.GetAllAsync();
                var nTPServerDTO = _mapper.Map<IEnumerable<NTPServerDTO>>(nTPServers);

                return nTPServerDTO;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<NTPServerDTO> GetNTPServerById(int id)
        {
            try
            {
                var nTPServer = await _unitOfWork.NTPServerRepository.GetByIdAsync(id);
                var nTPServerDTO = _mapper.Map<NTPServerDTO>(nTPServer);

                return nTPServerDTO;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public bool NTPServerExists(NTPServerDTO nTPServer)
        {
            throw new NotImplementedException();
        }

        public async Task SoftDeleteAndSave(HttpContext httpContext, int id)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                NTPServer nTPServer = new NTPServer();

                nTPServer = await _unitOfWork.NTPServerRepository.GetByIdAsync(id);

                NTPServerHistory serverHistory = _mapper.Map<NTPServerHistory>(nTPServer);

                serverHistory.IdHistory = serverHistory.Id;
                serverHistory.Id = 0;

                await _unitOfWork.NTPServerHistoryRepository.Insert(serverHistory);

                await _unitOfWork.SaveChangesAsync();

                nTPServer.DeleteDate = DateTime.Now;
                //Get current logged-in userId
                nTPServer.DeleteUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);

                await _unitOfWork.NTPServerRepository.SoftDelete(id);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitAsync();
            }
            catch (System.Exception)
            {
                await _unitOfWork.RollBackAsync();
            }
        }

        public async Task UpdateAndSave(HttpContext httpContext, NTPServerDTO Server)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                NTPServer nTPServer = new NTPServer();

                nTPServer = await _unitOfWork.NTPServerRepository.GetByIdAsync(Server.Id);

                NTPServerHistory serverHistory = _mapper.Map<NTPServerHistory>(nTPServer);

                serverHistory.IdHistory = serverHistory.Id;
                serverHistory.Id = 0;

                await _unitOfWork.NTPServerHistoryRepository.Insert(serverHistory);
                await _unitOfWork.SaveChangesAsync();

                nTPServer.ServerUrl = Server.ServerUrl;
                nTPServer.EditDate = DateTime.Now;
                //Get current logged-in userId
                nTPServer.EditUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);

                _unitOfWork.NTPServerRepository.Update(nTPServer);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitAsync();
            }
            catch (System.Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }
    }
}