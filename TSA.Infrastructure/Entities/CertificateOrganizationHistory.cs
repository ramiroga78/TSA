using System;
using System.Collections.Generic;
using TSA.Infrastructure.Entities;

#nullable disable

namespace TSALibrary.Models
{
    public partial class CertificateOrganizationHistory : BaseEntity
    {
        public int Id { get; set; }
        public int IdHistory { get; set; }
        public int IdCertificateOrganizationType { get; set; }
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
    }
}
