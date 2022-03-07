using System;
using System.Collections.Generic;
using TSA.Infrastructure.Entities;

#nullable disable

namespace TSALibrary.Models
{
    public partial class CertificateOrganization : BaseEntity
    {
        public CertificateOrganization()
        {
            CertificateIssuers = new HashSet<Certificate>();
            CertificateSubjects = new HashSet<Certificate>();
        }

        public int Id { get; set; }
        public int CertificateOrganizationTypeId { get; set; }
        public string CommonName { get; set; }
        public string OrganizationName { get; set; }
        public string CountryName { get; set; }
        public int AddUserId { get; set; }
        public int? EditUserId { get; set; }
        public int? DeleteUserId { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime? EditDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public bool IsActive { get; set; }

        public virtual CertificateOrganizationType CertificateOrganizationType { get; set; }
        public virtual ICollection<Certificate> CertificateIssuers { get; set; }
        public virtual ICollection<Certificate> CertificateSubjects { get; set; }
        //public virtual ICollection<Certificate> Certificates { get; set; }
        
    }
}
