using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TSA.Infrastructure.DTOs;
using TSA.Infrastructure.Interfaces;
using TSA.Infrastructure.ViewModels;
using TSALibrary.Models;

namespace TSA.Infrastructure.Services
{
    public class CertificateService : BaseService, ICertificateService
    {
        private new readonly IUnitOfWork _unitOfWork;
        private new readonly IMapper _mapper;
        public CertificateService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddAndSave(HttpContext httpContext, CertificatesVM certificateVM)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                Certificate certificate = _mapper.Map<Certificate>(certificateVM.Certificate);

                certificate.IsActive = true;

                await _unitOfWork.CertificateRepository.Insert(certificate);

                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitAsync();
            }
            catch (System.Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<CertificateDTO>> GetAllCertificates()
        {
            try
            {
                var certificates = await _unitOfWork.CertificateRepository.GetAllAsync();
                var certificatesDto = _mapper.Map<IEnumerable<CertificateDTO>>(certificates);

                return certificatesDto;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<ICollection<CertificatesVM>> GetAllCertificatesVM()
        {
            try
            {
                var certificates = await _unitOfWork.CertificateRepository.GetAllAsync();

                ICollection<CertificatesVM> certificatesVM = new List<CertificatesVM>();

                foreach (Certificate certificate in certificates)
                {
                    CertificatesVM certificateVM = new CertificatesVM();

                    CertificateOrganization certificateOrganization = await _unitOfWork.CertificateOrganizationRepository.GetByIdAsync(certificate.IssuerId);

                    certificateVM.Certificate = certificate;
                    certificateVM.CommonName = certificateOrganization.CommonName;

                    certificatesVM.Add(certificateVM);
                }

                return certificatesVM;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<Certificate> GetCertificateById(int id)
        {
            try
            {
                var certificate = await _unitOfWork.CertificateRepository.GetByIdAsync(id);

                return certificate;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<Certificate> GetCertificateByIdIncludingProfilesAndPolicies(int id)
        {
            try
            {
                var certificate = await _unitOfWork.CertificateRepository.GetCertificateByIdIncludingProfilesAndPolicies(id);

                return certificate;
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
                Certificate certificate = new Certificate();

                certificate = await _unitOfWork.CertificateRepository.GetByIdAsync(id);

                CertificatesHistory certificateHistory = _mapper.Map<CertificatesHistory>(certificate);

                certificateHistory.IdHistory = certificateHistory.Id;
                certificateHistory.Id = 0;

                await _unitOfWork.CertificatesHistoryRepository.Insert(certificateHistory);

                await _unitOfWork.SaveChangesAsync();

                certificate.DeleteDate = DateTime.Now;
                //Get current logged-in userId
                certificate.DeleteUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);

                await _unitOfWork.CertificateRepository.SoftDelete(id);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitAsync();
            }
            catch (System.Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        public async Task UpdateAndSave(HttpContext httpContext, CertificatesVM certVM, ICollection<CertificatePolicy> certPolicies, ICollection<CertificateProfile> certProfiles)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                Certificate cert = new Certificate();

                cert = await _unitOfWork.CertificateRepository.GetByIdAsync(certVM.Certificate.Id);

                Certificate certTemp = new Certificate();

                certTemp = await this.GetCertificateByIdIncludingProfilesAndPolicies(certVM.Certificate.Id);

                //----------------------------- Guardar historico de Policies ----------------------------
                ICollection<CertificatePoliciesHistory> certPoliciesHistory = new Collection<CertificatePoliciesHistory>();

                foreach (CertificatePolicy certpol in certTemp.CertificatePolicies)
                {
                    CertificatePoliciesHistory certPolHistory = new CertificatePoliciesHistory();

                    certPolHistory.IdHistory = certpol.Id;
                    certPolHistory.IdPolicyType = certpol.IdPolicyType;
                    certPolHistory.IdCertificate = certpol.CertificateId;
                    certPolHistory.PolicyValue = certpol.PolicyValue;
                    certPolHistory.AddUserId = certpol.AddUserId;
                    certPolHistory.AddDate = certpol.AddDate;

                    await _unitOfWork.CertificatePoliciesHistoryRepository.Insert(certPolHistory);

                    _unitOfWork.CertificatePolicyRepository.Delete(certpol);

                    await _unitOfWork.SaveChangesAsync();
                }

                certVM.Certificate.CertificatePolicies.Clear();

                foreach (CertificatePolicy certPol in certPolicies)
                {
                    await _unitOfWork.CertificatePolicyRepository.Insert(certPol);
                    await _unitOfWork.SaveChangesAsync();
                }
                //----------------------------------------------------------------------------------------

                //----------------------------- Guardar historico de Profiles ----------------------------
                ICollection<CertificateProfilesHistory> certProfilesHistory = new Collection<CertificateProfilesHistory>();

                foreach (CertificateProfile certprof in certTemp.CertificateProfiles)
                {
                    CertificateProfilesHistory certProfHistory = new CertificateProfilesHistory();

                    certProfHistory.IdHistory = certprof.Id;
                    certProfHistory.IdCertificate = certprof.CertificateId;
                    certProfHistory.IdProfileType = certprof.ProfileTypeId;
                    certProfHistory.IdProfileValue = certprof.IdProfileValue;
                    certProfHistory.AddUserId = certprof.AddUserId;
                    certProfHistory.AddDate = certprof.AddDate;

                    await _unitOfWork.CertificateProfilesHistoryRepository.Insert(certProfHistory);

                    _unitOfWork.CertificateProfileRepository.Delete(certprof);

                    await _unitOfWork.SaveChangesAsync();
                }

                certVM.Certificate.CertificateProfiles.Clear();

                foreach (CertificateProfile certProf in certProfiles)
                {
                    await _unitOfWork.CertificateProfileRepository.Insert(certProf);
                    await _unitOfWork.SaveChangesAsync();
                }
                //---------------------------------------------------------------------------------------

                CertificatesHistory certHistory = _mapper.Map<CertificatesHistory>(cert);

                certHistory.IdHistory = certHistory.Id;
                certHistory.Id = 0;

                await _unitOfWork.CertificatesHistoryRepository.Insert(certHistory);
                await _unitOfWork.SaveChangesAsync();

                cert.SerialNumber = certVM.Certificate.SerialNumber;
                cert.IsDefault = certVM.Certificate.IsDefault;
                cert.IssuerId = certVM.Certificate.IssuerId;
                cert.SubjectId = certVM.Certificate.SubjectId;
                cert.Thumbprint = certVM.Certificate.Thumbprint;
                cert.NotBefore = certVM.Certificate.NotBefore;
                cert.NotAfter = certVM.Certificate.NotAfter;
                cert.EditDate = DateTime.Now;
                //Get current logged-in userId
                cert.EditUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);

                _unitOfWork.CertificateRepository.Update(cert);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitAsync();
            }
            catch (System.Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        public async Task<Certificate> ExistsCertWithPolicy( string policy)
        {
           var list = await _unitOfWork.CertificateRepository.GetAllAsync(x => x.CertificatePolicies.Any(y => y.PolicyValue.Equals(policy)), null, "CertificatePolicies");
            return list.FirstOrDefault();
        }

        public async Task<Certificate> ReturnDefault()
        {
            var list2 = await _unitOfWork.CertificateRepository.GetAllAsync(x => x.IsDefault == true, null, "CertificatePolicies");
            return list2.FirstOrDefault();
        }
    }
}