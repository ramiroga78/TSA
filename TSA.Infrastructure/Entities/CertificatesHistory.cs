using System;
using System.Collections.Generic;
using TSA.Infrastructure.Entities;

#nullable disable

namespace TSALibrary.Models
{
    public partial class CertificatesHistory : BaseEntity
    {
        public int Id { get; set; }
        public int IdHistory { get; set; }
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
    }
}
