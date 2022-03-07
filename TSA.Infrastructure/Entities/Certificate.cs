using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TSA.Infrastructure.Entities;

#nullable disable

namespace TSALibrary.Models
{
    public partial class Certificate : BaseEntity
    {
        public Certificate()
        {
            CertificatePolicies = new HashSet<CertificatePolicy>();
            CertificateProfiles = new HashSet<CertificateProfile>();
        }

        public int Id { get; set; }
        //[ForeignKey("CertificateOrganization")]
        public int IssuerId { get; set; }
        //[ForeignKey("CertificateOrganization")]
        public int? SubjectId { get; set; }
        public string SerialNumber { get; set; }
        public string Thumbprint { get; set; }
        public DateTime NotBefore { get; set; }
        public DateTime NotAfter { get; set; }
        public bool? IsDefault { get; set; }
        public int AddUserId { get; set; }
        public int? EditUserId { get; set; }
        public int? DeleteUserId { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime? EditDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public bool IsActive { get; set; }

        public virtual CertificateOrganization Issuer { get; set; }
        public virtual CertificateOrganization Subject { get; set; }
        public virtual ICollection<CertificatePolicy> CertificatePolicies { get; set; }
        public virtual ICollection<CertificateProfile> CertificateProfiles { get; set; }
    }
}
