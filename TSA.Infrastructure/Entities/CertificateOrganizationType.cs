using System;
using System.Collections.Generic;
using TSA.Infrastructure.Entities;

#nullable disable

namespace TSALibrary.Models
{
    public partial class CertificateOrganizationType : BaseEntity
    {
        public CertificateOrganizationType()
        {
            CertificateOrganizations = new HashSet<CertificateOrganization>();
        }

        public int Id { get; set; }
        public string Description { get; set; }

        public virtual ICollection<CertificateOrganization> CertificateOrganizations { get; set; }
    }
}
