using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TSA.Infrastructure.Entities;

#nullable disable

namespace TSALibrary.Models
{
    public partial class RoleUser : BaseEntity
    {
        public int Id { get; set; }
        public int RoleId { get; set; }        
        public int UserId { get; set; }
        public int AddUserId { get; set; }
        public DateTime AddDate { get; set; }
        public virtual Role Roles { get; set; }
        public virtual User Users { get; set; }
    }
}
