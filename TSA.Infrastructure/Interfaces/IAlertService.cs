using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using TSA.Infrastructure.DTOs;

namespace TSA.Infrastructure.Interfaces
{
    public interface IAlertService
    {
        public Task<IEnumerable<AlertDTO>> GetAllAlerts();
        public Task<IEnumerable<AlertUserDTO>> GetAllUsersByAlertIdAndModelToDto(int alertId);
        public AlertUserDTO CreateAlertUserDto();
        public Task CreateAlertUser(HttpContext httpContext, AlertUserDTO alertUserDto);
        public Task<IEnumerable<AlertUserDTO>> GetAllUsersAndModelToDto(int alertId);
        public Task<AlertUserDTO> GetUserAndModelToAlertUserDtoById(int Id);
        public Task UpdateAndSave(HttpContext httpContext, AlertUserDTO alertUserDto);
        public Task DeleteAlertUser(HttpContext httpContext, int id);
        public Task<AlertDTO> GetAlertById(int id);
    }
}
