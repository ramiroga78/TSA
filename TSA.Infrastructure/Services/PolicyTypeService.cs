using AutoMapper;
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
    public class PolicyTypeService : BaseService, IPolicyTypeService
    {
        public PolicyTypeService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        public async Task AddAndSave(HttpContext httpContext, PolicyTypeDTO policyTypeDto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var policyType = _mapper.Map<PoliciesType>(policyTypeDto);
                //Get current logged-in userId
                policyType.AddUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                policyType.IsActive = true;
                policyType.AddDate = DateTime.Now;
                await _unitOfWork.PolicyTypeRepository.Insert(policyType);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (System.Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }
        public PolicyTypeDTO CreatePolicyTypeDTO()
        {
            try
            {
                PolicyTypeDTO policyTypeDto = new PolicyTypeDTO();
                return policyTypeDto;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<PolicyTypeDTO>> GetAllPoliciesTypes()
        {
            try
            {
                var policiesTypes = await _unitOfWork.PolicyTypeRepository.GetAllAsync();
                var policiesTypesDto = _mapper.Map<IEnumerable<PolicyTypeDTO>>(policiesTypes);
                return policiesTypesDto;
            }
            catch (System.Exception)
            {
                throw;
            }
        }


        public async Task<PolicyTypeDTO> GetPolicyTypeByIdAndModelToDto(int id)
        {
            try
            {
                var policyType = await _unitOfWork.PolicyTypeRepository.GetPolicyTypeById(id);
                var policyTypeDto = _mapper.Map<PolicyTypeDTO>(policyType);
                return policyTypeDto;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task SoftDeleteAndSave(HttpContext httpContext, int id)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var policyType = await _unitOfWork.PolicyTypeRepository.GetPolicyTypeById(id);
                //Se guardan cambios en Histórico
                PoliciesTypesHistory policiesTypesHistory = new PoliciesTypesHistory();
                policiesTypesHistory.IdHistory = policyType.Id;
                policiesTypesHistory.Description = policyType.Description;
                policiesTypesHistory.AddUserId = policyType.AddUserId;
                policiesTypesHistory.EditUserId = policyType.EditUserId;
                policiesTypesHistory.DeleteUserId = policyType.DeleteUserId;
                policiesTypesHistory.AddDate = policyType.AddDate;
                policiesTypesHistory.EditDate = policyType.EditDate;
                policiesTypesHistory.DeleteDate = policyType.DeleteDate;
                policiesTypesHistory.IsActive = (bool)policyType.IsActive;
                await _unitOfWork.PolicyTypeHistoryRepository.Insert(policiesTypesHistory);
                //Se guardan cambios en PoliciesType
                policyType.IsActive = false;
                policyType.DeleteDate = System.DateTime.Now;
                //Get current logged-in userId
                policyType.DeleteUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                _unitOfWork.PolicyTypeRepository.Update(policyType);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (System.Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        public async Task UpdateAndSave(HttpContext httpContext, PolicyTypeDTO policyTypeDto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var policyType = await _unitOfWork.PolicyTypeRepository.GetPolicyTypeById(policyTypeDto.Id);
                //Se guardan cambios en Histórico
                PoliciesTypesHistory policiesTypesHistory = new PoliciesTypesHistory();
                policiesTypesHistory.IdHistory = policyType.Id;
                policiesTypesHistory.Description = policyType.Description;
                policiesTypesHistory.AddUserId = policyType.AddUserId;
                policiesTypesHistory.EditUserId = policyType.EditUserId;
                policiesTypesHistory.DeleteUserId = policyType.DeleteUserId;
                policiesTypesHistory.AddDate = policyType.AddDate;
                policiesTypesHistory.EditDate = policyType.EditDate;
                policiesTypesHistory.DeleteDate = policyType.DeleteDate;
                policiesTypesHistory.IsActive = (bool)policyType.IsActive;
                await _unitOfWork.PolicyTypeHistoryRepository.Insert(policiesTypesHistory);
                //Se guardan cambios en PoliciesType
                policyType.Description = policyTypeDto.Description;
                //Get current logged-in userId
                policyType.EditUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                policyType.EditDate = DateTime.Now;
                _unitOfWork.PolicyTypeRepository.Update(policyType);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (System.Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        public IEnumerable<SelectListItem> GetPoliciesTypesList()
        {
            try
            {
                return _unitOfWork.PolicyTypeRepository.GetPoliciesTypesList();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

    }
}
