using System;
using System.ComponentModel.DataAnnotations;
using TSA.Infrastructure.Entities;

namespace TSALibrary.Models
{
    public class ExternalUserHistory : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public int IdHistory { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int AddUserId { get; set; }
        public int? EditUserId { get; set; }
        public int? DeleteUserId { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime? EditDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public bool IsActive { get; set; }
    }
}
