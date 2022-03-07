using System;
using System.Collections.Generic;
using TSA.Infrastructure.Entities;

#nullable disable

namespace TSALibrary.Models
{
    public partial class CertificatePolicy:BaseEntity
    {
        public int Id { get; set; }
        public int IdPolicyType { get; set; }
        public int CertificateId { get; set; }
        public string PolicyValue { get; set; }
        public int AddUserId { get; set; }
        public DateTime AddDate { get; set; }

        public virtual Certificate Certificate { get; set; }
        public virtual PoliciesType PolicyType { get; set; }
    }
}
