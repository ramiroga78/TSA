using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TSA.Infrastructure.DTOs;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;

namespace TSA.Infrastructure.Services
{
    class CertificateOrganizationService : BaseService, ICertificateOrganizationService
    {
        public CertificateOrganizationService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public async Task AddAndSave(HttpContext httpContext, CertificateOrganizationDTO certificateOrganizationDTO)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var certificateOrganization = _mapper.Map<CertificateOrganization>(certificateOrganizationDTO);
                //Get current logged-in userId
                certificateOrganization.AddUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                certificateOrganization.IsActive = true;
                certificateOrganization.AddDate = DateTime.Now;
                await _unitOfWork.CertificateOrganizationRepository.Insert(certificateOrganization);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (System.Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        public Task<bool> CertificateOrganizationExists(CertificateOrganizationDTO certificateOrganization)
        {
            throw new NotImplementedException();
        }

        public CertificateOrganizationDTO CreateCertificateOrganizationDto()
        {
            try
            {
                var certificateOrganizationDto = new CertificateOrganizationDTO();

                return certificateOrganizationDto;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<CertificateOrganizationDTO>> GetAllCertificateOrganization()
        {
            try
            {
                var certificatesOrganization = await _unitOfWork.CertificateOrganizationRepository.GetAllAsync();
                var certificatesOrganizationDto = _mapper.Map<IEnumerable<CertificateOrganizationDTO>>(certificatesOrganization);

                foreach (var item in certificatesOrganizationDto)
                {
                    item.CertificateOrganizationType = await _unitOfWork.CertificateOrganizationTypeRepository.
                                                                    GetCertificateOrganizationTypeById(item.CertificateOrganizationTypeId);
                }

                return certificatesOrganizationDto;
            }
            catch (System.Exception)
            {
                throw;
            }
        }


        public async Task<CertificateOrganizationDTO> GetCertificateOrganizationByIdAndModelToDto(int id)
        {
            try
            {
                var certificateOrganization = await _unitOfWork.CertificateOrganizationRepository.GetCertificateOrganizationById(id);

                if (certificateOrganization != null)
                {
                    var certificateOrganizationDto = _mapper.Map<CertificateOrganizationDTO>(certificateOrganization);
                    certificateOrganizationDto.CertificateOrganizationType = await _unitOfWork.CertificateOrganizationTypeRepository.
                                                                                GetCertificateOrganizationTypeById(certificateOrganizationDto.CertificateOrganizationTypeId);
                    certificateOrganizationDto.CountryList = GetCountryList();

                    return certificateOrganizationDto;
                }
            }
            catch (System.Exception)
            {
                throw;
            }

            return null;
        }

        public async Task SoftDeleteAndSave(HttpContext httpContext, int id)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var certificateOrganization = await _unitOfWork.CertificateOrganizationRepository.GetCertificateOrganizationById(id);
                //Se guardan cambios en Histórico
                CertificateOrganizationHistory certificateOrganizationHistory = new CertificateOrganizationHistory();
                certificateOrganizationHistory.IdHistory = certificateOrganization.Id;
                certificateOrganizationHistory.IdCertificateOrganizationType = certificateOrganization.CertificateOrganizationTypeId;
                certificateOrganizationHistory.CommonName = certificateOrganization.CommonName;
                certificateOrganizationHistory.OrganizationName = certificateOrganization.OrganizationName;
                certificateOrganizationHistory.CountryName = certificateOrganization.CountryName;
                certificateOrganizationHistory.AddUserId = certificateOrganization.AddUserId;
                certificateOrganizationHistory.EditUserId = certificateOrganization.EditUserId;
                certificateOrganizationHistory.DeleteUserId = certificateOrganization.DeleteUserId;
                certificateOrganizationHistory.AddDate = certificateOrganization.AddDate;
                certificateOrganizationHistory.EditDate = certificateOrganization.EditDate;
                certificateOrganizationHistory.DeleteDate = certificateOrganization.DeleteDate;
                certificateOrganizationHistory.IsActive = certificateOrganization.IsActive;
                await _unitOfWork.CertificateOrganizationHistoryRepository.Insert(certificateOrganizationHistory);
                //Se guardan cambios en CertificateOrganization
                certificateOrganization.IsActive = false;
                certificateOrganization.DeleteDate = System.DateTime.Now;
                //Get current logged-in userId
                certificateOrganization.DeleteUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                _unitOfWork.CertificateOrganizationRepository.Update(certificateOrganization);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (System.Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        public async Task UpdateAndSave(HttpContext httpContext, CertificateOrganizationDTO certificateOrganizationDTO)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var certificateOrganization = await _unitOfWork.CertificateOrganizationRepository.
                                                    GetCertificateOrganizationById(certificateOrganizationDTO.Id);
                //Se guardan cambios en Histórico
                CertificateOrganizationHistory certificateOrganizationHistory = new CertificateOrganizationHistory();
                certificateOrganizationHistory.IdHistory = certificateOrganization.Id;
                certificateOrganizationHistory.IdCertificateOrganizationType = certificateOrganization.CertificateOrganizationTypeId;
                certificateOrganizationHistory.CommonName = certificateOrganization.CommonName;
                certificateOrganizationHistory.OrganizationName = certificateOrganization.OrganizationName;
                certificateOrganizationHistory.CountryName = certificateOrganization.CountryName;
                certificateOrganizationHistory.AddUserId = certificateOrganization.AddUserId;
                certificateOrganizationHistory.EditUserId = certificateOrganization.EditUserId;
                certificateOrganizationHistory.DeleteUserId = certificateOrganization.DeleteUserId;
                certificateOrganizationHistory.AddDate = certificateOrganization.AddDate;
                certificateOrganizationHistory.EditDate = certificateOrganization.EditDate;
                certificateOrganizationHistory.DeleteDate = certificateOrganization.DeleteDate;
                certificateOrganizationHistory.IsActive = certificateOrganization.IsActive;
                await _unitOfWork.CertificateOrganizationHistoryRepository.Insert(certificateOrganizationHistory);
                //Se guardan cambios en CertificateOrganization
                certificateOrganization.CommonName = certificateOrganizationDTO.CommonName;
                certificateOrganization.CountryName = certificateOrganizationDTO.CountryName;
                certificateOrganization.OrganizationName = certificateOrganizationDTO.OrganizationName;
                certificateOrganization.CertificateOrganizationTypeId = certificateOrganizationDTO.CertificateOrganizationTypeId;
                //Get current logged-in userId
                certificateOrganization.EditUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                certificateOrganization.EditDate = DateTime.Now;
                _unitOfWork.CertificateOrganizationRepository.Update(certificateOrganization);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (System.Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<CertificateOrganizationType>> GetAllCertificateOrganizationTypes()
        {
            try
            {
                var certificateOrganizationTypes = await _unitOfWork.CertificateOrganizationTypeRepository.GetAllAsync();

                return certificateOrganizationTypes;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<CertificateOrganizationType> GetCertificateOrganizationTypeById(int id)
        {
            try
            {
                var certificateOrganizationTypes = await _unitOfWork.CertificateOrganizationTypeRepository.GetCertificateOrganizationTypeById(id);

                return certificateOrganizationTypes;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        //GET ALL COUNTRIES

        public List<string> GetCountryList()
        {
            try
            {
                List<string> cultureList = new List<string>();

                CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

                foreach (CultureInfo culture in cultures)
                {
                    RegionInfo region = new RegionInfo(culture.LCID);

                    if (!(cultureList.Contains(region.EnglishName)))
                    {
                        cultureList.Add(region.EnglishName);
                    }
                }

                return cultureList;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public IEnumerable<SelectListItem> GetAllCertificateOrganizationByType(string type)
        {
            try
            {
                return _unitOfWork.CertificateOrganizationRepository.GetAllCertificateOrganizationByType(type);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<CertificateOrganization> GetCertificateOrganizationById(int id)
        {
            try
            {
                CertificateOrganization certificateOrganization = await _unitOfWork.CertificateOrganizationRepository.GetCertificateOrganizationById(id);

                return certificateOrganization;
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}

