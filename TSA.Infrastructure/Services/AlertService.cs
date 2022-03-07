using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata;
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
    public class AlertService : BaseService, IAlertService
    {
        public AlertService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public async Task<IEnumerable<AlertDTO>> GetAllAlerts()
        {
            try
            {
                var alerts = await _unitOfWork.AlertRepository.GetAllAsync();
                var alertsDto = _mapper.Map<IEnumerable<AlertDTO>>(alerts);

                foreach (var alertDto in alertsDto)
                {
                    var users = _unitOfWork.AlertUserRepository.GetUsersByAlertId(alertDto.Id);
                    foreach (var user in users)
                    {
                        if (user.IsActive == true)
                        {
                            alertDto.UserCount++;
                        }
                    }
                }
                return alertsDto;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<AlertUserDTO>> GetAllUsersByAlertIdAndModelToDto(int alertId)
        {
            try
            {
                var alertUsers = _unitOfWork.AlertUserRepository.GetUsersByAlertId(alertId);
                var alertUsersDto = new List<AlertUserDTO>();

                foreach (var alertUser in alertUsers)
                {
                    if (alertUser.IsActive == true)
                    {
                        var alertUserDto = new AlertUserDTO();
                        var user = await _unitOfWork.UserRepository.GetByIdAsync(alertUser.UserId);

                        alertUserDto.Id = alertUser.Id;
                        alertUserDto.UserId = alertUser.UserId;
                        alertUserDto.AlertId = alertId;
                        alertUserDto.Name = user.Name;
                        alertUserDto.IsActive = alertUser.IsActive;
                        alertUsersDto.Add(alertUserDto);
                    }
                }
                return alertUsersDto;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<AlertUserDTO>> GetAllUsersAndModelToDto(int alertId)
        {
            try
            {
                var usersThatNoHaveAlertId = _unitOfWork.AlertUserRepository.GetAllUserThatNotHaveAlertById(alertId);
                var alertUsersDto = new List<AlertUserDTO>();

                foreach (var user in usersThatNoHaveAlertId)
                {
                    if (user.IsActive == true)
                    {
                        var alertUserDto = new AlertUserDTO();

                        alertUserDto.UserId = user.Id;
                        alertUserDto.AlertId = alertId;
                        alertUserDto.Name = user.Name;
                        alertUsersDto.Add(alertUserDto);
                    }
                }
                return alertUsersDto;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public AlertUserDTO CreateAlertUserDto()
        {
            try
            {
                var alertUserDto = new AlertUserDTO();

                return alertUserDto;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<AlertUserDTO> GetUserAndModelToAlertUserDtoById(int Id)
        {
            try
            {
                var alertUser = await _unitOfWork.AlertUserRepository.GetByIdAsync(Id);
                var userDto = await _unitOfWork.UserRepository.GetByIdAsync(alertUser.UserId);
                var alertUserDto = new AlertUserDTO { Id = alertUser.Id, AlertId = alertUser.AlertId, UserId = alertUser.UserId, Name = userDto.Name, IsActive = alertUser.IsActive };

                return alertUserDto;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task CreateAlertUser(HttpContext httpContext, AlertUserDTO alertUserDto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var alertUser = _unitOfWork.AlertUserRepository.GetAlertUserByAlertIdAndUserId(alertUserDto.AlertId, alertUserDto.UserId);
                if (alertUser == null)
                {
                    var alertUserEntity = new AlertUser();
                    alertUserEntity.AlertId = alertUserDto.AlertId;
                    alertUserEntity.UserId = alertUserDto.UserId;
                    //Get current logged-in userId
                    alertUserEntity.AddUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                    alertUserEntity.AddDate = DateTime.Now;
                    alertUserEntity.IsActive = true;
                    await _unitOfWork.AlertUserRepository.Insert(alertUserEntity);
                }
                else
                {
                    //GUARDO CAMBIOS EN HISTÓRICO
                    var alertUserHistory = new AlertUserHistory();
                    alertUserHistory.IdHistory = alertUser.Id;
                    alertUserHistory.AlertId = alertUser.AlertId;
                    alertUserHistory.UserId = alertUser.UserId;
                    alertUserHistory.AddUserId = alertUser.AddUserId;
                    alertUserHistory.EditUserId = alertUser.EditUserId;
                    alertUserHistory.DeleteUserId = alertUser.DeleteUserId;
                    alertUserHistory.AddDate = alertUser.AddDate;
                    alertUserHistory.EditDate = alertUser.EditDate;
                    alertUserHistory.DeleteDate = alertUser.DeleteDate;
                    alertUserHistory.IsActive = (bool)alertUser.IsActive;
                    await _unitOfWork.AlertUserHistoryRepository.Insert(alertUserHistory);
                    //GUARDO CAMBIOS EN ALERTUSER
                    //Get current logged-in userId
                    alertUser.EditUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                    alertUser.EditDate = DateTime.Now;
                    alertUser.IsActive = true;
                    _unitOfWork.AlertUserRepository.Update(alertUser);
                }
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

            }
            catch (System.Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        //RECIBO ALERTUSERDTO. ME TRAIGO ALERTUSER ANTES DE SER MODIFICADA (VIEJA) Y ME TRAIGO LA MODIFICADA (NUEVA)
        //GUARDO EN HISTÓRICO LOS CAMBIOS DE LA ALERTUSER VIEJA Y ACTUALIZO SU TABLA PARA QUE PASE A SER INACTIVA
        //LUEGO, SI LA NUEVA NO TENIA UN REGISTRO PROPIO, SE GENERA. SI NO, SE GUARDARN LOS CAMBIOS EN
        //HISTÓRICO Y SE ACTUALIZA SU TABLA PARA QUE PASE A SER ACTIVA
        public async Task UpdateAndSave(HttpContext httpContext, AlertUserDTO alertUserDto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var alertUserOld = await _unitOfWork.AlertUserRepository.GetByIdAsync(alertUserDto.Id);
                var alertUserNew = _unitOfWork.AlertUserRepository.GetAlertUserByAlertIdAndUserId(alertUserDto.AlertId, alertUserDto.UserId);
                //GUARDO CAMBIOS EN HISTÓRICO DE OLD
                var alertUserHistoryOld = new AlertUserHistory();
                alertUserHistoryOld.IdHistory = alertUserOld.Id;
                alertUserHistoryOld.AlertId = alertUserOld.AlertId;
                alertUserHistoryOld.UserId = alertUserOld.UserId;
                alertUserHistoryOld.AddUserId = alertUserOld.AddUserId;
                alertUserHistoryOld.EditUserId = alertUserOld.EditUserId;
                alertUserHistoryOld.DeleteUserId = alertUserOld.DeleteUserId;
                alertUserHistoryOld.AddDate = alertUserOld.AddDate;
                alertUserHistoryOld.EditDate = alertUserOld.EditDate;
                alertUserHistoryOld.DeleteDate = alertUserOld.DeleteDate;
                alertUserHistoryOld.IsActive = (bool)alertUserOld.IsActive;
                await _unitOfWork.AlertUserHistoryRepository.Insert(alertUserHistoryOld);
                //GUARDO CAMBIOS EN OLD       
                alertUserOld.IsActive = false;
                //Get current logged-in userId
                alertUserOld.EditUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                alertUserOld.EditDate = DateTime.Now;
                _unitOfWork.AlertUserRepository.Update(alertUserOld);

                if (alertUserNew == null)
                {
                    var alertUserEntity = new AlertUser();
                    alertUserEntity.AlertId = alertUserDto.AlertId;
                    alertUserEntity.UserId = alertUserDto.UserId;
                    //Get current logged-in userId
                    alertUserEntity.AddUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                    alertUserEntity.AddDate = (DateTime)alertUserOld.EditDate;
                    alertUserEntity.IsActive = true;
                    await _unitOfWork.AlertUserRepository.Insert(alertUserEntity);
                }
                else
                {
                    //GUARDO CAMBIOS EN HISTÓRICO DE NEW
                    var alertUserHistoryNew = new AlertUserHistory();
                    alertUserHistoryNew.IdHistory = alertUserNew.Id;
                    alertUserHistoryNew.AlertId = alertUserNew.AlertId;
                    alertUserHistoryNew.UserId = alertUserNew.UserId;
                    alertUserHistoryNew.AddUserId = alertUserNew.AddUserId;
                    alertUserHistoryNew.EditUserId = alertUserNew.EditUserId;
                    alertUserHistoryNew.DeleteUserId = alertUserNew.DeleteUserId;
                    alertUserHistoryNew.AddDate = alertUserNew.AddDate;
                    alertUserHistoryNew.EditDate = alertUserNew.EditDate;
                    alertUserHistoryNew.DeleteDate = alertUserNew.DeleteDate;
                    alertUserHistoryNew.IsActive = (bool)alertUserNew.IsActive;
                    await _unitOfWork.AlertUserHistoryRepository.Insert(alertUserHistoryNew);
                    //GUARDO CAMBIOS EN NEW
                    alertUserNew.IsActive = true;
                    //Get current logged-in userId
                    alertUserNew.EditUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                    alertUserNew.EditDate = alertUserOld.EditDate;
                    _unitOfWork.AlertUserRepository.Update(alertUserNew);
                }
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (System.Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        public async Task DeleteAlertUser(HttpContext httpContext, int id)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var alertUser = await _unitOfWork.AlertUserRepository.GetByIdAsync(id);
                //GUARDO CAMBIOS EN HISTÓRICO
                var alertUserHistory = new AlertUserHistory();
                alertUserHistory.IdHistory = alertUser.Id;
                alertUserHistory.AlertId = alertUser.AlertId;
                alertUserHistory.UserId = alertUser.UserId;
                alertUserHistory.AddUserId = alertUser.AddUserId;
                alertUserHistory.EditUserId = alertUser.EditUserId;
                alertUserHistory.DeleteUserId = alertUser.DeleteUserId;
                alertUserHistory.AddDate = alertUser.AddDate;
                alertUserHistory.EditDate = alertUser.EditDate;
                alertUserHistory.DeleteDate = alertUser.DeleteDate;
                alertUserHistory.IsActive = (bool)alertUser.IsActive;
                await _unitOfWork.AlertUserHistoryRepository.Insert(alertUserHistory);
                //GUARDO CAMBIOS EN ALERTUSER
                alertUser.IsActive = false;
                alertUser.DeleteDate = DateTime.Now;
                //Get current logged-in userId
                alertUser.DeleteUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                _unitOfWork.AlertUserRepository.Update(alertUser);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (System.Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        public async Task<AlertDTO> GetAlertById(int id)
        {
            try
            {
                Alert alert = await _unitOfWork.AlertRepository.GetAlertById(id);

                AlertDTO alertDTO = _mapper.Map<AlertDTO>(alert);

                return alertDTO;
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}
