using System;
using TSA.Infrastructure.Entities;

#nullable disable

namespace TSALibrary.Models
{
    public partial class ProfileValueHistory : BaseEntity
    {
        public int Id { get; set; }
        public int IdHistory { get; set; }
        public int IdProfileValueHistory { get; set; }
        public string Value { get; set; }
        public int AddUserId { get; set; }
        public int? EditUserId { get; set; }
        public int? DeleteUserId { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime? EditDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public bool? IsActive { get; set; }
    }
}
