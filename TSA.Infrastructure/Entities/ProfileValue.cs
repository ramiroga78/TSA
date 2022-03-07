using System;
using System.Collections.Generic;
using TSA.Infrastructure.Entities;

#nullable disable

namespace TSALibrary.Models
{
    public partial class ProfileValue : BaseEntity
    {
        public ProfileValue()
        {
            CertificateProfiles = new HashSet<CertificateProfile>();
        }
        public int Id { get; set; }
        public int ProfileTypeId { get; set; }
        public string Value { get; set; }
        public int AddUserId { get; set; }
        public int? EditUserId { get; set; }
        public int? DeleteUserId { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime? EditDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<CertificateProfile> CertificateProfiles { get; set; }
        public virtual ProfileType ProfileType { get; set; }
    }
}
