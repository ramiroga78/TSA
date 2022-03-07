using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TSA.Infrastructure.DTOs;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.ViewModels;
using TSALibrary.Models;

namespace TSA.Infrastructure.Interfaces
{
    public interface ICertificateService
    {
        public Task<IEnumerable<CertificateDTO>> GetAllCertificates();

        public Task<ICollection<CertificatesVM>> GetAllCertificatesVM();
        public Task<Certificate> GetCertificateById(int id);
        public Task AddAndSave(HttpContext httpContext, CertificatesVM certificate);
        public Task UpdateAndSave(HttpContext httpContext, CertificatesVM certificate, ICollection<CertificatePolicy> certPolicies, ICollection<CertificateProfile> certProfiles);
        public Task SoftDeleteAndSave(HttpContext httpContext, int id);
        public Task<Certificate> GetCertificateByIdIncludingProfilesAndPolicies(int id);
        public Task<Certificate> ExistsCertWithPolicy(string policy);
        public Task<Certificate> ReturnDefault();

    }
}
