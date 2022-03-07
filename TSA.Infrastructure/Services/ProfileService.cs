using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using TSA.Infrastructure.DTOs;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace TSA.Infrastructure.Services
{
    public class ProfileService : BaseService, IProfileService
    {
        public ProfileService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        public ProfileTypeDTO CreateProfileTypeDTO()
        {
            ProfileTypeDTO profileTypeDTO = new ProfileTypeDTO();
            return profileTypeDTO;
        }
        public ProfileValueDTO CreateProfileValueDTO()
        {
            ProfileValueDTO profileValueDTO = new ProfileValueDTO();

            return profileValueDTO;
        }
        public async Task<IEnumerable<ProfileTypeDTO>> GetAllProfileTypes()
        {
            var profileTypes = await _unitOfWork.ProfileTypeRepository.GetAllAsync();
            var profileTypesDTO = _mapper.Map<IEnumerable<ProfileTypeDTO>>(profileTypes);

            return profileTypesDTO;
        }
        public async Task<IEnumerable<ProfileValueDTO>> GetAllProfileValues()
        {
            var profileValues = await _unitOfWork.ProfileValueRepository.GetAllAsync();
            var profileValuesDTO = _mapper.Map<IEnumerable<ProfileValueDTO>>(profileValues);

            return profileValuesDTO;
        }
        public async Task<ProfileTypeDTO> GetProfileTypeByIdAndModelToDto(int id)
        {
            var profileType = await _unitOfWork.ProfileTypeRepository.GetProfileTypeById(id);
            var profileTypeDTO = _mapper.Map<ProfileTypeDTO>(profileType);

            return profileTypeDTO;
        }
        public async Task<ProfileValueDTO> GetProfileValueByIdAndModelToDto(int Id)
        {
            var profileValue = await _unitOfWork.ProfileValueRepository.GetProfileValueById(Id);
            var profileValueDTO = _mapper.Map<ProfileValueDTO>(profileValue);

            return profileValueDTO;
            ;
        }
        public async Task <IEnumerable<ProfileValueDTO>> GetProfileValuesByTypeId(int typeId)
        {
            var profilevalues = await _unitOfWork.ProfileValueRepository.GetAllAsync(x => x.ProfileTypeId == typeId);
            var profileValueDTO = _mapper.Map<IEnumerable<ProfileValueDTO>>(profilevalues);

            return profileValueDTO;
        }
        public async Task AddAndSaveType(HttpContext httpContext, ProfileTypeDTO profileTypeDTO)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var profileType = _mapper.Map<ProfileType>(profileTypeDTO);
                profileType.AddUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value); 
                profileType.IsActive = true;
                profileType.AddDate = DateTime.Now;

                await _unitOfWork.ProfileTypeRepository.Insert(profileType);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (System.Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }
        public async Task UpdateAndSaveType(HttpContext httpContext, ProfileTypeDTO profileTypeDTO)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var profileType = await _unitOfWork.ProfileTypeRepository.GetProfileTypeById(profileTypeDTO.Id);

                //ProfileType
                profileType.Description = profileTypeDTO.Description;
                profileType.EditUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                profileType.EditDate = DateTime.Now;
                profileType.IsActive = true;


                _unitOfWork.ProfileTypeRepository.Update(profileType);

                //ProfileTypeHistory
                ProfileTypesHistory profileTypesHistory = new ProfileTypesHistory();

                profileTypesHistory.IdHistory = profileType.Id;
                profileTypesHistory.Description = profileType.Description;
                profileTypesHistory.AddUserId = profileType.AddUserId;
                profileTypesHistory.EditUserId = profileType.EditUserId;
                profileTypesHistory.DeleteUserId = profileType.DeleteUserId;
                profileTypesHistory.AddDate = profileType.AddDate;
                profileTypesHistory.EditDate = profileType.EditDate;
                profileTypesHistory.DeleteDate = profileType.DeleteDate;
                profileTypesHistory.IsActive = (bool)profileType.IsActive;

                await _unitOfWork.ProfileTypeHistoryRepository.Insert(profileTypesHistory);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (System.Exception ex)
            {
                await _unitOfWork.RollBackAsync();
                //log
            }

        }
        public async Task SoftDeleteAndSaveType(HttpContext httpContext, int id)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var profileType = await _unitOfWork.ProfileTypeRepository.GetProfileTypeById(id);

                //ProfileType
                profileType.DeleteUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                profileType.DeleteDate = DateTime.Now;
                profileType.IsActive = false;

                _unitOfWork.ProfileTypeRepository.Update(profileType);

                //ProfileTypeHistory
                ProfileTypesHistory profileTypesHistory = new ProfileTypesHistory();

                profileTypesHistory.IdHistory = profileType.Id;
                profileTypesHistory.Description = profileType.Description;
                profileTypesHistory.AddUserId = profileType.AddUserId;
                profileTypesHistory.EditUserId = profileType.EditUserId;
                profileTypesHistory.DeleteUserId = profileType.DeleteUserId;
                profileTypesHistory.AddDate = profileType.AddDate;
                profileTypesHistory.EditDate = profileType.EditDate;
                profileTypesHistory.DeleteDate = profileType.DeleteDate;
                profileTypesHistory.IsActive = (bool)profileType.IsActive;

                await _unitOfWork.ProfileTypeHistoryRepository.Insert(profileTypesHistory);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();


            }
            catch (System.Exception ex)
            {
                await _unitOfWork.RollBackAsync();
                //log
            }

        }
        public async Task AddAndSaveValue(HttpContext httpContext,ProfileValueDTO profileValueDTO)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var profileValue = _mapper.Map<ProfileValue>(profileValueDTO);

                profileValue.Id = 0;
                profileValue.Value = profileValueDTO.Value;
                profileValue.ProfileTypeId = profileValueDTO.ProfileTypeId;
                profileValue.AddUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                profileValue.AddDate = DateTime.Now;
                profileValue.IsActive = true;

                await _unitOfWork.ProfileValueRepository.Insert(profileValue);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (System.Exception ex)
            {

                await _unitOfWork.RollBackAsync();
                throw;
            }
        }
        public async Task UpdateAndSaveValue(HttpContext httpContext, ProfileValueDTO profileValueDTO)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var profileValue = await _unitOfWork.ProfileValueRepository.GetProfileValueById(profileValueDTO.Id);

                //ProfileValue
                profileValue.Value = profileValueDTO.Value;
                profileValue.EditUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                profileValue.EditDate = DateTime.Now;
                profileValue.IsActive = true;

                _unitOfWork.ProfileValueRepository.Update(profileValue);

                //ProfileValueHistory
                ProfileValueHistory profileValueHistory = new ProfileValueHistory();

                profileValueHistory.IdHistory = profileValue.Id;
                profileValueHistory.Value = profileValue.Value;
                profileValueHistory.AddUserId = profileValue.AddUserId;
                profileValueHistory.EditUserId = profileValue.EditUserId;
                profileValueHistory.DeleteUserId = profileValue.DeleteUserId;
                profileValueHistory.AddDate = profileValue.AddDate;
                profileValueHistory.EditDate = profileValue.EditDate;
                profileValueHistory.DeleteDate = profileValue.DeleteDate;
                profileValueHistory.IsActive = (bool)profileValue.IsActive;

                await _unitOfWork.ProfileValueHistoryRepository.Insert(profileValueHistory);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();


            }
            catch (System.Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }

        }
        public async Task SoftDeleteAndSaveValue(HttpContext httpContext, int id)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var profileValue = await _unitOfWork.ProfileValueRepository.GetProfileValueById(id);

                //ProfileValue
                profileValue.DeleteUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                profileValue.DeleteDate = DateTime.Now;
                profileValue.IsActive = false;

                //ProfileValueHistory
                ProfileValueHistory profileValueHistory = new ProfileValueHistory();

                profileValueHistory.IdHistory = profileValue.Id;
                profileValueHistory.Value = profileValue.Value;
                profileValueHistory.AddUserId = profileValue.AddUserId;
                profileValueHistory.EditUserId = profileValue.EditUserId;
                profileValueHistory.DeleteUserId = profileValue.DeleteUserId;
                profileValueHistory.AddDate = profileValue.AddDate;
                profileValueHistory.EditDate = profileValue.EditDate;
                profileValueHistory.DeleteDate = profileValue.DeleteDate;
                profileValueHistory.IsActive = (bool)profileValue.IsActive;

                await _unitOfWork.ProfileValueHistoryRepository.Insert(profileValueHistory);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();


            }
            catch (System.Exception ex)
            {
                await _unitOfWork.RollBackAsync();
                //log
            }
        }


        public IEnumerable<SelectListItem> GetProfilesTypeList()
        {
            try
            {
                return _unitOfWork.ProfileTypeRepository.GetProfilesTypesList();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public IEnumerable<SelectListItem> GetProfilesValuesList()
        {
            try
            {
                return _unitOfWork.ProfileValueRepository.GetProfilesValuesList();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public IList<ProfileValue> GetProfilesValuesByTypeList(int profileTypeId)
        {
            try
            {
                return _unitOfWork.ProfileValueRepository.GetProfilesValuesByTypeList(profileTypeId);
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}

