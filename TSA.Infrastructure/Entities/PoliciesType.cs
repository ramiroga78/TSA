using System;
using System.Collections.Generic;
using TSA.Infrastructure.Entities;

#nullable disable

namespace TSALibrary.Models
{
    public partial class PoliciesType : BaseEntity
    {
        public PoliciesType()
        {
            CertificatePolicies = new HashSet<CertificatePolicy>();
        }
        public int Id { get; set; }
        public string Description { get; set; }
        public int AddUserId { get; set; }
        public int? EditUserId { get; set; }
        public int? DeleteUserId { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime? EditDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<CertificatePolicy> CertificatePolicies { get; set; }
    }
}
