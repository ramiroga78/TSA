using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using TSA.Infrastructure.DTOs;
using TSALibrary.Models;

namespace TSA.Infrastructure.Interfaces
{
    public interface ICertificateOrganizationService
    {
        public Task<IEnumerable<CertificateOrganizationType>> GetAllCertificateOrganizationTypes();
        public Task<IEnumerable<CertificateOrganizationDTO>> GetAllCertificateOrganization();
        public Task<CertificateOrganizationType> GetCertificateOrganizationTypeById(int id);
        public Task<CertificateOrganizationDTO> GetCertificateOrganizationByIdAndModelToDto(int id);
        public Task UpdateAndSave(HttpContext httpContext, CertificateOrganizationDTO certificateOrganizationDto);
        public Task SoftDeleteAndSave(HttpContext httpContext, int id);
        public Task<bool> CertificateOrganizationExists(CertificateOrganizationDTO certificateOrganizationDto);
        public Task AddAndSave(HttpContext httpContext, CertificateOrganizationDTO certificateOrganizationDto);
        public CertificateOrganizationDTO CreateCertificateOrganizationDto();
        public List<string> GetCountryList();
        public IEnumerable<SelectListItem> GetAllCertificateOrganizationByType(string type);
        public Task<CertificateOrganization> GetCertificateOrganizationById(int id);
    }
}
