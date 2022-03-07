using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TSALibrary.Models;

namespace TSA.Infrastructure.DTOs
{
    public class CertificateDTO
    {
        public CertificateDTO()
        {
            CertificatePolicies = new HashSet<CertificatePolicy>();
            CertificateProfiles = new HashSet<CertificateProfile>();
        }

        public int Id { get; set; }
        public int IdIssuer { get; set; }
        public int? IdSubject { get; set; }
        public string SerialNumber { get; set; }
        public string Thumbprint { get; set; }
        public DateTime? NotBefore { get; set; }
        public DateTime? NotAfter { get; set; }
        public bool? IsDefault { get; set; }
        public int AddUserId { get; set; }
        public int? EditUserId { get; set; }
        public int? DeleteUserId { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime? EditDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public bool IsActive { get; set; }

        public virtual CertificateOrganization IdIssuerNavigation { get; set; }
        public virtual CertificateOrganization IdSubjectNavigation { get; set; }
        public virtual ICollection<CertificatePolicy> CertificatePolicies { get; set; }
        public virtual ICollection<CertificateProfile> CertificateProfiles { get; set; }
    }
}
