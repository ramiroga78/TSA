using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using TSA.Infrastructure.DTOs;

namespace TSA.Infrastructure.Interfaces
{
    public interface IPolicyTypeService
    {
        public Task AddAndSave(HttpContext httpContext, PolicyTypeDTO policyTypeDto);
        public PolicyTypeDTO CreatePolicyTypeDTO();
        public Task<IEnumerable<PolicyTypeDTO>> GetAllPoliciesTypes();
        public Task<PolicyTypeDTO> GetPolicyTypeByIdAndModelToDto(int id);
        public Task SoftDeleteAndSave(HttpContext httpContext, int id);
        public Task UpdateAndSave(HttpContext httpContext, PolicyTypeDTO policyTypeDto);

        public IEnumerable<SelectListItem> GetPoliciesTypesList();
    }
}
