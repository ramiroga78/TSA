using System;
using System.Collections.Generic;
using TSA.Infrastructure.Entities;

#nullable disable

namespace TSALibrary.Models
{
    public partial class CertificateProfile : BaseEntity
    {
        public int Id { get; set; }
        public int CertificateId { get; set; }
        public int ProfileTypeId { get; set; }
        public int? IdProfileValue { get; set; }
        public int AddUserId { get; set; }
        public DateTime AddDate { get; set; }

        public virtual Certificate Certificate { get; set; }
        public virtual ProfileType ProfileType { get; set; }
        public virtual ProfileValue ProfileValue { get; set; }
    }
}
