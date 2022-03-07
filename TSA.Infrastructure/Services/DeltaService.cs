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
    public class DeltaService : BaseService, IDeltaService
    {
        public DeltaService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        //DELTA
        public DeltaDTO CreateDeltaDTO()
        {
            var deltaDTO = new DeltaDTO();
            return deltaDTO;
        }
        public async Task<IEnumerable<DeltaDTO>> GetAllDeltas()
        {
            IEnumerable<Delta> deltas = await _unitOfWork.DeltaRepository.GetAllAsync();
            IEnumerable <DeltaDTO> deltasDTO = _mapper.Map<IEnumerable<DeltaDTO>>(deltas);

            foreach (DeltaDTO deltaDTO in deltasDTO)
            {
                IEnumerable<DeltaUser> users = _unitOfWork.DeltaUserRepository.GetUsersByDeltaId(deltaDTO.Id);
                
                foreach (DeltaUser user in users)
                {
                    if (user.IsActive == true)
                    {
                        deltaDTO.UserCount++;
                    }
                }
            }
            return deltasDTO;
        }
        public async Task<DeltaDTO> GetAllDeltaByIdAndModelToDto(int id)
        {
            var delta = await _unitOfWork.DeltaRepository.GetDeltaById(id);
            var deltaDTO = _mapper.Map<DeltaDTO>(delta);
            return deltaDTO;
        }
        public async Task AddAndSave(HttpContext httpContext, DeltaDTO deltaDTO)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var delta = _mapper.Map<Delta>(deltaDTO);
                delta.AddUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                delta.IsActive = true;
                delta.AddDate = DateTime.Now;

                await _unitOfWork.DeltaRepository.Insert(delta);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (System.Exception ex)
            {
                await _unitOfWork.RollBackAsync();
            }
        }
        public async Task UpdateAndSave(HttpContext httpContext, DeltaDTO deltaDTO)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var delta = await _unitOfWork.DeltaRepository.GetDeltaById(deltaDTO.Id);
                //DELTA
                delta.ControlOperator = deltaDTO.ControlOperator;
                delta.ControlOperatorValue = deltaDTO.ControlOperatorValue;
                delta.StopService = deltaDTO.StopService;
                delta.EditUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                delta.EditDate = DateTime.Now;
                delta.IsActive = true;
                _unitOfWork.DeltaRepository.Update(delta);
                //DELTA HISTORY
                DeltaHistory deltaHistory = new DeltaHistory();
                deltaHistory.IdHistory = delta.Id;
                deltaHistory.DeltaTypeId = delta.DeltaTypeId;
                deltaHistory.EventCode = delta.EventCode;
                deltaHistory.EventName = delta.EventName;
                deltaHistory.EventDescription = delta.EventDescription;
                deltaHistory.StopService = delta.StopService;
                deltaHistory.ControlOperator = delta.ControlOperator;
                deltaHistory.ControlOperatorValue = delta.ControlOperatorValue;
                deltaHistory.AddUserId = delta.AddUserId;
                deltaHistory.EditUserId = delta.EditUserId;
                deltaHistory.DeleteUserId = delta.DeleteUserId;
                deltaHistory.AddDate = delta.AddDate;
                deltaHistory.EditDate = delta.EditDate;
                deltaHistory.DeleteUserId = delta.DeleteUserId;
                deltaHistory.IsActive = delta.IsActive;
                await _unitOfWork.DeltaHistoryRepository.Insert(deltaHistory);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (System.Exception ex)
            {
                await _unitOfWork.RollBackAsync();
            }
        }
        //DELTA TYPES
        public async Task<DeltaTypeDTO> GetDeltaType(int typeId)
        {
            var deltaType = await  _unitOfWork.DeltaTypeRepository.GetByIdAsync(typeId);
            var deltaTypeDTO = new DeltaTypeDTO { Id = deltaType.Id, Description = deltaType.Description, IsActive = deltaType.IsActive };

            return deltaTypeDTO;
        }
        //DELTA USERS
        public DeltaUserDTO CreateDeltaUserDTO()
        {
            var deltaUserDTO = new DeltaUserDTO();
            return deltaUserDTO;
        }
        public async Task<IEnumerable<DeltaUserDTO>> GetAllUsersByDeltaIdAndModelToDto(int deltaId)
        {
            var deltaUsers = _unitOfWork.DeltaUserRepository.GetUsersByDeltaId(deltaId);
            var deltaUsersDTO = new List<DeltaUserDTO>();
            foreach (var deltaUser in deltaUsers)
            {
                var deltaUserDTO = new DeltaUserDTO();
                var user = await _unitOfWork.UserRepository.GetByIdAsync(deltaUser.UserId);
                deltaUserDTO.Id = deltaUser.Id;
                deltaUserDTO.UserId = deltaUser.UserId;
                deltaUserDTO.DeltaId = deltaUser.DeltaId;
                deltaUserDTO.Name = user.Name;
                deltaUserDTO.IsActive = deltaUser.IsActive;
                deltaUsersDTO.Add(deltaUserDTO);
            }
            return deltaUsersDTO;
        }
        public async Task<IEnumerable<DeltaUserDTO>> GetAllUsersAndModelToDto(int deltaId)
        {
            var usersThatNoHaveDeltaId = _unitOfWork.DeltaUserRepository.GetAllUserThatNotHaveDeltaById(deltaId);
            var deltaUsersDTO = new List<DeltaUserDTO>();
            foreach (var user in usersThatNoHaveDeltaId)
            {
                var deltaUserDTO = new DeltaUserDTO();
                deltaUserDTO.UserId = user.Id;
                deltaUserDTO.DeltaId = deltaId;
                deltaUserDTO.Name = user.Name;
                deltaUsersDTO.Add(deltaUserDTO);
            }
            return deltaUsersDTO;
        }
        public async Task<DeltaUserDTO> GetUserAndModelToDeltaUserDtoById(int id)
        {
            var deltaUser = await _unitOfWork.DeltaUserRepository.GetByIdAsync(id);
            var userDTO = await _unitOfWork.UserRepository.GetByIdAsync(deltaUser.UserId);
            var deltaUserDTO = new DeltaUserDTO { Id = deltaUser.Id, DeltaId = deltaUser.DeltaId, UserId = deltaUser.UserId, Name = userDTO.Name, IsActive = deltaUser.IsActive };
            return deltaUserDTO;
        }
        public async Task CreateDeltaUser(HttpContext httpContext, DeltaUserDTO deltaUserDTO)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var deltaUser = _unitOfWork.DeltaUserRepository.GetDeltaUserByDeltaIdAndDeltaId(deltaUserDTO.DeltaId, deltaUserDTO.UserId);
                if (deltaUser == null)
                {
                    var deltaUserEntity = new DeltaUser();
                    deltaUserEntity.DeltaId = deltaUserDTO.DeltaId;
                    deltaUserEntity.UserId = deltaUserDTO.UserId;
                    deltaUserEntity.AddUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                    deltaUserEntity.AddDate = DateTime.Now;
                    deltaUserEntity.IsActive = true;
                    await _unitOfWork.DeltaUserRepository.Insert(deltaUserEntity);
                }
                else
                {
                    deltaUser.EditUserId = 1;
                    deltaUser.EditDate = DateTime.Now;
                    deltaUser.IsActive = true;
                    _unitOfWork.DeltaUserRepository.Update(deltaUser);
                    var deltaUserHistory = new DeltaUserHistory();
                    deltaUserHistory.IdHistory = deltaUser.Id;
                    deltaUserHistory.DeltaId = deltaUser.DeltaId;
                    deltaUserHistory.UserId = deltaUser.UserId;
                    deltaUserHistory.AddUserId = deltaUser.AddUserId;
                    deltaUserHistory.EditUserId = deltaUser.EditUserId;
                    deltaUserHistory.DeleteUserId = deltaUser.DeleteUserId;
                    deltaUserHistory.AddDate = deltaUser.AddDate;
                    deltaUserHistory.EditDate = deltaUser.EditDate;
                    deltaUserHistory.DeleteDate = deltaUser.DeleteDate;
                    deltaUserHistory.IsActive = deltaUser.IsActive;

                    await _unitOfWork.DeltaUserHistoryRepository.Insert(deltaUserHistory);
                }
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (System.Exception ex)
            {
                await _unitOfWork.RollBackAsync();
                //_logger.Log(ex.Message)            
            }
        }
        public async Task UpdateAndSaveDeltaUser(HttpContext httpContext, DeltaUserDTO deltaUserDTO)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var deltaUserOld = await _unitOfWork.DeltaUserRepository.GetByIdAsync(deltaUserDTO.Id);
                var deltaUserNew = _unitOfWork.DeltaUserRepository.GetDeltaUserByDeltaIdAndDeltaId(deltaUserDTO.DeltaId, deltaUserDTO.UserId);

                deltaUserOld.EditUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                deltaUserOld.EditDate = DateTime.Now;
                deltaUserOld.IsActive = false;

                _unitOfWork.DeltaUserRepository.Update(deltaUserOld);

                var deltaUserHistoryOld = new DeltaUserHistory();

                deltaUserHistoryOld.IdHistory = deltaUserOld.Id;
                deltaUserHistoryOld.DeltaId = deltaUserOld.DeltaId;
                deltaUserHistoryOld.UserId = deltaUserOld.UserId;
                deltaUserHistoryOld.AddUserId = deltaUserOld.AddUserId;
                deltaUserHistoryOld.EditUserId = deltaUserOld.EditUserId;
                deltaUserHistoryOld.DeleteUserId = deltaUserOld.DeleteUserId;
                deltaUserHistoryOld.AddDate = deltaUserOld.AddDate;
                deltaUserHistoryOld.EditDate = deltaUserOld.EditDate;
                deltaUserHistoryOld.DeleteDate = deltaUserOld.DeleteDate;
                deltaUserHistoryOld.IsActive = deltaUserOld.IsActive;

                await _unitOfWork.DeltaUserHistoryRepository.Insert(deltaUserHistoryOld);

                if (deltaUserNew == null)
                {
                    var deltaUserEntity = new DeltaUser();

                    deltaUserEntity.DeltaId = deltaUserDTO.DeltaId;
                    deltaUserEntity.UserId = deltaUserDTO.UserId;
                    deltaUserEntity.AddUserId = 1;
                    deltaUserEntity.AddDate = DateTime.Now;
                    deltaUserEntity.IsActive = true;

                    await _unitOfWork.DeltaUserRepository.Insert(deltaUserEntity);
                }
                else
                {
                    deltaUserNew.EditUserId = 1;
                    deltaUserNew.EditDate = DateTime.Now;
                    deltaUserNew.IsActive = true;

                    _unitOfWork.DeltaUserRepository.Update(deltaUserNew);

                    var deltaUserHistoryNew = new DeltaUserHistory();

                    deltaUserHistoryNew.IdHistory = deltaUserNew.Id;
                    deltaUserHistoryNew.DeltaId = deltaUserNew.DeltaId;
                    deltaUserHistoryNew.UserId = deltaUserNew.UserId;
                    deltaUserHistoryNew.AddUserId = deltaUserNew.AddUserId;
                    deltaUserHistoryNew.EditUserId = deltaUserNew.EditUserId;
                    deltaUserHistoryNew.DeleteUserId = deltaUserNew.DeleteUserId;
                    deltaUserHistoryNew.AddDate = deltaUserNew.AddDate;
                    deltaUserHistoryNew.EditDate = deltaUserNew.EditDate;
                    deltaUserHistoryNew.DeleteDate = deltaUserNew.DeleteDate;
                    deltaUserHistoryNew.IsActive = deltaUserNew.IsActive;

                    await _unitOfWork.DeltaUserHistoryRepository.Insert(deltaUserHistoryNew);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (System.Exception ex)
            {
                await _unitOfWork.RollBackAsync();
                //_logger.Log(ex.Message)            
            }
        }
        public async Task DeleteDeltaUser(HttpContext httpContext, int id)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var deltaUser = await _unitOfWork.DeltaUserRepository.GetByIdAsync(id);

                deltaUser.DeleteUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                deltaUser.DeleteDate = DateTime.Now;
                deltaUser.IsActive = false;

                _unitOfWork.DeltaUserRepository.Update(deltaUser);

                var deltaUserHistory = new DeltaUserHistory();

                deltaUserHistory.IdHistory = deltaUser.Id;
                deltaUserHistory.DeltaId = deltaUser.DeltaId;
                deltaUserHistory.UserId = deltaUser.UserId;
                deltaUserHistory.AddUserId = deltaUser.AddUserId;
                deltaUserHistory.EditUserId = deltaUser.EditUserId;
                deltaUserHistory.DeleteUserId = deltaUser.DeleteUserId;
                deltaUserHistory.AddDate = deltaUser.AddDate;
                deltaUserHistory.EditDate = deltaUser.EditDate;
                deltaUserHistory.DeleteDate = deltaUser.DeleteDate;
                deltaUserHistory.IsActive = deltaUser.IsActive;

                await _unitOfWork.DeltaUserHistoryRepository.Insert(deltaUserHistory);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (System.Exception ex)
            {
                await _unitOfWork.RollBackAsync();
                //_logger.Log(ex.Message)            
            }
        }
    }
}

