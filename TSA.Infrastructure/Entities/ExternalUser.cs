using System;
using System.ComponentModel.DataAnnotations.Schema;
using TSA.Infrastructure.Entities;

namespace TSALibrary.Models
{
    public class ExternalUser : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        [NotMapped] // Does not effect with your database
        public string ActualPassword { get; set; }
        [NotMapped] // Does not effect with your database
        public string ConfirmPassword { get; set; }
        public int AddUserId { get; set; }
        public int? EditUserId { get; set; }
        public int? DeleteUserId { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime? EditDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public bool IsActive { get; set; }
    }
}
