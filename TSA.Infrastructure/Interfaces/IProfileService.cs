using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using TSA.Infrastructure.DTOs;
using TSALibrary.Models;
using Microsoft.AspNetCore.Http;

namespace TSA.Infrastructure.Interfaces
{
    public interface IProfileService
    {
        public ProfileTypeDTO CreateProfileTypeDTO();
        public ProfileValueDTO CreateProfileValueDTO();
        public Task<IEnumerable<ProfileTypeDTO>> GetAllProfileTypes();
        public Task<IEnumerable<ProfileValueDTO>> GetAllProfileValues();
        public Task<ProfileTypeDTO> GetProfileTypeByIdAndModelToDto(int id);
        public Task<IEnumerable<ProfileValueDTO>> GetProfileValuesByTypeId(int typeId);
        public Task<ProfileValueDTO> GetProfileValueByIdAndModelToDto(int valueId);
        public Task AddAndSaveType(HttpContext httpContext, ProfileTypeDTO profileTypeDTO);
        public Task AddAndSaveValue(HttpContext httpContext, ProfileValueDTO profileValueDTO);
        public Task UpdateAndSaveType(HttpContext httpContext, ProfileTypeDTO profileTypeDTO);
        public Task UpdateAndSaveValue(HttpContext httpContext, ProfileValueDTO profileValueDTO);
        public Task SoftDeleteAndSaveType(HttpContext httpContext, int id);
        public Task SoftDeleteAndSaveValue(HttpContext httpContext, int id);

        public IEnumerable<SelectListItem> GetProfilesTypeList();
        public IEnumerable<SelectListItem> GetProfilesValuesList();
        public IList<ProfileValue> GetProfilesValuesByTypeList(int profileTypeId);
    }
}
