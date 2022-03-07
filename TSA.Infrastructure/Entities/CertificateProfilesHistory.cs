using System;
using System.Collections.Generic;
using TSA.Infrastructure.Entities;

#nullable disable

namespace TSALibrary.Models
{
    public partial class CertificateProfilesHistory : BaseEntity
    {
        public int Id { get; set; }
        public int IdHistory { get; set; }
        public int IdCertificate { get; set; }
        public int IdProfileType { get; set; }
        public int? IdProfileValue { get; set; }
        public int AddUserId { get; set; }
        public DateTime AddDate { get; set; }
    }
}
