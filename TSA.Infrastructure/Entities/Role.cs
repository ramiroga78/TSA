using System;
using System.Collections.Generic;
using TSA.Infrastructure.Entities;

#nullable disable

namespace TSALibrary.Models
{
    public partial class Role : BaseEntity
    {
        public Role()
        {
            RoleUsers = new HashSet<RoleUser>();
            RolesFunctions = new HashSet<RoleFunction>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int AddUserId { get; set; }
        public int? EditUserId { get; set; }
        public int? DeleteUserId { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime? EditDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<RoleUser> RoleUsers { get; set; }
        public virtual ICollection<RoleFunction> RolesFunctions { get; set; }
    }
}
