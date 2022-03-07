using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using TSA.Infrastructure.DTOs;
using Microsoft.AspNetCore.Http;

namespace TSA.Infrastructure.Interfaces
{
    public interface IRoleService
    {
        public Task<IEnumerable<RoleDTO>> GetAllRoles();
        public Task<RoleDTO> GetRoleByIdAndModelToDto(int id);
        public Task UpdateAndSave(HttpContext httpContext, RoleDTO role);
        public Task SoftDeleteAndSave(HttpContext httpContext, int id);
        public Task<bool> RoleExists(RoleDTO role);
        public Task AddAndSave(HttpContext httpContext, RoleDTO role);
        public RoleDTO CreateRoleDto();
        public List<RoleFunctionDTO> GetRoleFunctionsByIdAndModelToDto(int id);
        public Task ModifyRolesFunctionsAndSave(HttpContext httpContext, List<RoleFunctionDTO> roleFunctionsDto);
        
        public IEnumerable<SelectListItem> GetRolesList();
    }
}
