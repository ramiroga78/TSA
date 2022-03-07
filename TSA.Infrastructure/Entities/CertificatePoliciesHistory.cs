using System;
using System.Collections.Generic;
using TSA.Infrastructure.Entities;

#nullable disable

namespace TSALibrary.Models
{
    public partial class CertificatePoliciesHistory : BaseEntity
    {
        public int Id { get; set; }
        public int IdHistory { get; set; }
        public int IdPolicyType { get; set; }
        public int IdCertificate { get; set; }
        public string PolicyValue { get; set; }
        public int AddUserId { get; set; }
        public DateTime AddDate { get; set; }
    }
}
