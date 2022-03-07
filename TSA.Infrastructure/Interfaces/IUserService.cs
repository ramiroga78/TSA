using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using TSA.Infrastructure.DTOs;
using TSA.Infrastructure.Entities;
using TSALibrary.Models;

namespace TSA.Infrastructure.Interfaces
{
    public interface IUserService
    {
        public Task<IEnumerable<UserDTO>> GetAllUsers();
        public Task<User> GetUserById(int id);
        public Task AddAndSave(HttpContext httpContext, User user, int roleId);
        public Task UpdateAndSave(HttpContext httpContext, User user, int roleId);
        public Task SoftDeleteAndSave(HttpContext httpContext, int id);
        public Task<User> GetUserByEmail(string email);
        public Task<bool> UserExists(User user);
        public Task<int> GetUserRoleByUserId(int id);
        //--------------------------------------------------------------------------
        public Task<bool> SignIn(HttpContext httpContext, User userLogin, bool isPersistent = false);
        public Task SignOut(HttpContext httpContext);
        //--------------------------------------------------------------------------
        public Task ChangePassword(HttpContext httpContext, User user);
    }
}
