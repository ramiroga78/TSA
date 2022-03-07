using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using TSA.Infrastructure.DTOs;
using TSA.Infrastructure.Entities;
using TSALibrary.Models;

namespace TSA.Infrastructure.Interfaces
{
    public interface IExternalUserService
    {
        public Task<IEnumerable<ExternalUserDTO>> GetAllUsers();
        public Task<ExternalUser> GetUserById(int id);
        public Task<ExternalUserDTO> GetDTOUserById(int id);
        public Task<ExternalUser> GetUserByEmail(string email);
        public Task AddAndSave(HttpContext httpContext, ExternalUserDTO user);
        public Task UpdateAndSave(HttpContext httpContext, ExternalUserDTO user);
        public Task SoftDeleteAndSave(HttpContext httpContext, int id);
        public Task<bool> UserExists(ExternalUser user);
        public Task ChangePassword(HttpContext httpContext, ExternalUserDTO user);
    }
}
