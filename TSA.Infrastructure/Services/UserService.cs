using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using TSA.Infrastructure.DTOs;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;

namespace TSA.Infrastructure.Services
{
    public class UserService : BaseService, IUserService
    {
        public UserService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsers()
        {
            try
            {
                var users = await _unitOfWork.UserRepository.GetAllAsync();
                var usersDto = _mapper.Map<IEnumerable<UserDTO>>(users);
                //return usersDto;
                return usersDto;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<User> GetUserById(int id)
        {
            try
            {
                return await _unitOfWork.UserRepository.GetByIdAsyncIncludingRoles(id);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<int> GetUserRoleByUserId(int id)
        {
            try
            {
                return await _unitOfWork.RoleUserRepository.GetUserRoleByUserId(id);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public Task<User/*UserDTO*/> GetUserByEmail(string email)
        {
            try
            {
                return _unitOfWork.UserRepository.GetUserByEmail(email);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task AddAndSave(HttpContext httpContext, User user, int roleId)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                //Password encrypt
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                user.AddDate = DateTime.Now;
                //Get current logged-in userId
                user.AddUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                user.IsActive = true;

                RoleUser roleUser = new RoleUser();

                roleUser.AddDate = user.AddDate;
                roleUser.AddUserId = user.AddUserId;
                roleUser.RoleId = roleId;
                roleUser.UserId = user.Id;

                user.RoleUsers.Add(roleUser);

                await _unitOfWork.UserRepository.Insert(user);

                int result = await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitAsync();
            }
            catch (System.Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        public async Task<bool> UserExists(User user)
        {
            try
            {
                return await _unitOfWork.UserRepository.UserExists(user);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task UpdateAndSave(HttpContext httpContext, User user, int roleId)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                User usr = new User();

                usr = await _unitOfWork.UserRepository.GetByIdAsyncIncludingRoles(user.Id);

                UserHistory usrHistory = _mapper.Map<UserHistory>(usr);

                usrHistory.IdHistory = usrHistory.Id;
                usrHistory.Id = 0;

                await _unitOfWork.UserHistoryRepository.Insert(usrHistory);
                await _unitOfWork.SaveChangesAsync();

                usr.Name = user.Name;
                usr.Email = user.Email;
                usr.EditDate = DateTime.Now;
                //Get current logged-in userId
                usr.EditUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);

                if (usr.RoleUsers.Count == 0)
                // Sería un caso IMPOSIBLE porque al crear el usuario se tuvo que seleccionar un Rol
                {
                    await _unitOfWork.RollBackAsync();
                }
                else
                {
                    RoleUser roleUser = new RoleUser();
                    RoleUser newRoleUser = new RoleUser();
                    RoleUserHistory roleUserHistory = new RoleUserHistory();

                    roleUser = usr.RoleUsers.FirstOrDefault();

                    roleUserHistory = _mapper.Map<RoleUserHistory>(roleUser);

                    roleUserHistory.Id = 0;

                    await _unitOfWork.RoleUserHistoryRepository.Insert(roleUserHistory);
                    await _unitOfWork.SaveChangesAsync();

                    //Se elimina el rol anterior dado que solo puede tener 1 rol asignado
                    usr.RoleUsers.Remove(roleUser);
                    _unitOfWork.RoleUserRepository.Delete(roleUser);

                    await _unitOfWork.SaveChangesAsync();

                    newRoleUser.AddDate = (DateTime)usr.EditDate;
                    newRoleUser.AddUserId = (int)usr.EditUserId;
                    newRoleUser.RoleId = roleId;
                    newRoleUser.UserId = usr.Id;

                    usr.RoleUsers.Add(newRoleUser);

                    _unitOfWork.UserRepository.Update(usr);

                    await _unitOfWork.SaveChangesAsync();

                    await _unitOfWork.CommitAsync();
                }
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
                User usr = new User();

                usr = await _unitOfWork.UserRepository.GetByIdAsync(id);

                UserHistory usrHistory = _mapper.Map<UserHistory>(usr);

                usrHistory.IdHistory = usrHistory.Id;
                usrHistory.Id = 0;

                await _unitOfWork.UserHistoryRepository.Insert(usrHistory);

                await _unitOfWork.SaveChangesAsync();

                usr.DeleteDate = DateTime.Now;
                //Get current logged-in userId
                usr.DeleteUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);

                await _unitOfWork.UserRepository.SoftDelete(id);
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
        public async Task<bool> SignIn(HttpContext httpContext, User userLogin, bool isPersistent)
        {
            try
            {
                //User user = new User();
               var  user = await _unitOfWork.UserRepository.GetUserByEmail(userLogin.Email);

                if (user != null)
                {
                    bool passwordMatch = BCrypt.Net.BCrypt.Verify(userLogin.Password, user.Password);

                    if (user.Email.ToLower() == user.Email.ToLower() && passwordMatch && user.IsActive == true)
                    {
                        ClaimsIdentity identity = new ClaimsIdentity(this.GetUserClaims(user), CookieAuthenticationDefaults.AuthenticationScheme);
                        ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                        var props = new AuthenticationProperties();
                        props.IsPersistent = isPersistent;

                        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);

                        return true;
                    }
                }

                return false;
            }
            catch (System.Exception)
            {
                throw ;
            }
        }

        public async Task SignOut(HttpContext httpContext)
        {
            try
            {
                await httpContext.SignOutAsync();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        private IEnumerable<Claim> GetUserClaims(User user)
        {
            try
            {
                List<Claim> claims = new List<Claim>();

                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                claims.Add(new Claim(ClaimTypes.Name, user.Name));
                claims.Add(new Claim(ClaimTypes.Email, user.Email));
                claims.AddRange(this.GetUserRoleClaims(user));

                return claims;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        private IEnumerable<Claim> GetUserRoleClaims(User user)
        {
            try
            {
                List<Claim> claims = new List<Claim>();

                //claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));

                int roleId = this.GetUserRoleByUserId(user.Id).Result;
                Role role = _unitOfWork.RoleRepository.GetRoleById(roleId).Result;

                claims.Add(new Claim(ClaimTypes.Role, role.Name));

                //------------------------------------------------------
                var functions = _unitOfWork.FunctionRepository.GetAllFunctions();
                var rolesFunctions = _unitOfWork.RoleFunctionRepository.GetRoleFunctionsByRoleId(role.Id);
                foreach (var function in functions)
                {
                    foreach (var roleFunction in rolesFunctions)
                    {
                        if (roleFunction.FunctionId == function.Id)
                        {
                            //claims.Add(new Claim("Function", function.Name));
                            claims.Add(new Claim(function.Name + ".Create", roleFunction.Create.ToString()));
                            claims.Add(new Claim(function.Name + ".Read", roleFunction.Read.ToString()));
                            claims.Add(new Claim(function.Name + ".Update", roleFunction.Update.ToString()));
                            claims.Add(new Claim(function.Name + ".Delete", roleFunction.Delete.ToString()));
                        }
                    }
                }
                //------------------------------------------------------
                return claims;
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        //--------------------------------------------------------------------

        public async Task ChangePassword(HttpContext httpContext, User user)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                User usr = new User();

                usr = await _unitOfWork.UserRepository.GetByIdAsyncIncludingRoles(user.Id);

                UserHistory usrHistory = _mapper.Map<UserHistory>(usr);

                usrHistory.IdHistory = usrHistory.Id;
                usrHistory.Id = 0;

                await _unitOfWork.UserHistoryRepository.Insert(usrHistory);

                await _unitOfWork.SaveChangesAsync();

                usr.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                usr.EditDate = DateTime.Now;
                //Get current logged-in userId
                usr.EditUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);

                _unitOfWork.UserRepository.Update(usr);

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
