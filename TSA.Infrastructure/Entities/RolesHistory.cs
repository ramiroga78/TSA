using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TSA.Infrastructure.Entities;

#nullable disable

namespace TSALibrary.Models
{
    public partial class RolesHistory : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public int IdHistory { get; set; }
        public string Name { get; set; }
        public int AddUserId { get; set; }
        public int? EditUserId { get; set; }
        public int? DeleteUserId { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime? EditDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public bool IsActive { get; set; }
    }
}
