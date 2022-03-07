using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TSA.Infrastructure.Entities;

#nullable disable

namespace TSALibrary.Models
{
    public partial class RoleUserHistory : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public int RolesId { get; set; }
        public int UsersId { get; set; }
        public int AddUserId { get; set; }
        public DateTime AddDate { get; set; }
    }
}
