using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TSA.Infrastructure.Entities;

#nullable disable

namespace TSALibrary.Models
{
    public partial class User: BaseEntity
    {
        public User()
        {
            AlertUsers = new HashSet<AlertUser>();
            DeltaUsers = new HashSet<DeltaUser>();
            RoleUsers = new HashSet<RoleUser>();
        }

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

        public virtual ICollection<AlertUser> AlertUsers { get; set; }
        public virtual ICollection<DeltaUser> DeltaUsers { get; set; }
        public virtual ICollection<RoleUser> RoleUsers { get; set; }
    }
}
