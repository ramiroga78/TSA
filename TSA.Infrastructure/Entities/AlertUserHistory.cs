using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TSA.Infrastructure.Entities;

#nullable disable

namespace TSALibrary.Models
{
    public partial class AlertUserHistory : BaseEntity
    {
        public int Id { get; set; }
        public int IdHistory { get; set; }
        public int AlertId { get; set; }
        public int UserId { get; set; }
        public int AddUserId { get; set; }
        public int? EditUserId { get; set; }
        public int? DeleteUserId { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime? EditDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public bool? IsActive { get; set; }
    }
}
