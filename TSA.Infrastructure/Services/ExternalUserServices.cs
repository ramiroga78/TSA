using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TSA.Infrastructure.DTOs;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;

namespace TSA.Infrastructure.Services
{
    public class ExternalUserService : BaseService, IExternalUserService
    {
        public ExternalUserService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public async Task<IEnumerable<ExternalUserDTO>> GetAllUsers()
        {
            try
            {
                var users = await _unitOfWork.ExternalUserRepository.GetAllAsync();
                var usersDto = _mapper.Map<IEnumerable<ExternalUserDTO>>(users);
                //return usersDto;
                return usersDto;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<ExternalUser> GetUserById(int id)
        {
            try
            {
                return await _unitOfWork.ExternalUserRepository.GetByIdAsyncIncludingRoles(id);
            }
            catch (System.Exception)
            {
                throw;
            }
        }


        public async Task<ExternalUserDTO> GetDTOUserById(int id)
        {
            try
            {
                var extUser = await _unitOfWork.ExternalUserRepository.GetByIdAsyncIncludingRoles(id);
                var externalUserDTO = _mapper.Map<ExternalUserDTO>(extUser);
                return externalUserDTO;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public Task<ExternalUser> GetUserByEmail(string email)
        {
            try
            {
                return _unitOfWork.ExternalUserRepository.GetUserByEmail(email);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task AddAndSave(HttpContext httpContext, ExternalUserDTO usr)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                ExternalUser user = _mapper.Map<ExternalUser>(usr);
                //Password encrypt
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                user.AddDate = DateTime.Now;
                //Get current logged-in userId
                user.AddUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                user.IsActive = true;

                await _unitOfWork.ExternalUserRepository.Insert(user);

                int result = await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitAsync();
            }
            catch (System.Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        public async Task<bool> UserExists(ExternalUser user)
        {
            try
            {
                return await _unitOfWork.ExternalUserRepository.UserExists(user);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task UpdateAndSave(HttpContext httpContext, ExternalUserDTO user)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                ExternalUser usr = new ExternalUser();

                usr = await _unitOfWork.ExternalUserRepository.GetByIdAsync(user.Id);

                ExternalUserHistory usrHistory = _mapper.Map<ExternalUserHistory>(usr);

                usrHistory.IdHistory = usrHistory.Id;
                usrHistory.Id = 0;

                await _unitOfWork.ExternalUserHistoryRepository.Insert(usrHistory);
                await _unitOfWork.SaveChangesAsync();

                usr.Name = user.Name;
                usr.Email = user.Email;
                usr.EditDate = DateTime.Now;
                //Get current logged-in userId
                usr.EditUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                if (user.Password != null)
                {
                    usr.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                }

                _unitOfWork.ExternalUserRepository.Update(usr);

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
                ExternalUser usr = new ExternalUser();

                usr = await _unitOfWork.ExternalUserRepository.GetByIdAsync(id);

                ExternalUserHistory usrHistory = _mapper.Map<ExternalUserHistory>(usr);

                usrHistory.IdHistory = usrHistory.Id;
                usrHistory.Id = 0;

                await _unitOfWork.ExternalUserHistoryRepository.Insert(usrHistory);

                await _unitOfWork.SaveChangesAsync();

                usr.DeleteDate = DateTime.Now;
                //Get current logged-in userId
                usr.DeleteUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);

                await _unitOfWork.ExternalUserRepository.SoftDelete(id);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitAsync();
            }
            catch (System.Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        //--------------------------------------------------------------------
        public async Task<bool> Validate(string email, string password)
        {
            try
            {
                ExternalUser user = new ExternalUser();
                user = await _unitOfWork.ExternalUserRepository.GetUserByEmail(email);

                if (user != null)
                {
                    bool passwordMatch = BCrypt.Net.BCrypt.Verify(password, user.Password);

                    if (user.Email.ToLower() == user.Email.ToLower() && passwordMatch && user.IsActive == true)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task ChangePassword(HttpContext httpContext, ExternalUserDTO user)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                ExternalUser usr = new ExternalUser();

                usr = await _unitOfWork.ExternalUserRepository.GetByIdAsyncIncludingRoles(user.Id);

                ExternalUserHistory usrHistory = _mapper.Map<ExternalUserHistory>(usr);

                usrHistory.IdHistory = usrHistory.Id;
                usrHistory.Id = 0;

                await _unitOfWork.ExternalUserHistoryRepository.Insert(usrHistory);

                await _unitOfWork.SaveChangesAsync();

                usr.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                usr.EditDate = DateTime.Now;
                //Get current logged-in userId
                usr.EditUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);

                _unitOfWork.ExternalUserRepository.Update(usr);

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
