using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TSA.Infrastructure.Entities;

#nullable disable

namespace TSALibrary.Models
{
    public partial class RoleFunctionHistory : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public int FunctionId { get; set; }
        public int RoleId { get; set; }
        public int IdHistory { get; set; }
        public int AddUserId { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime EditDate { get; set; }
        public bool Create { get; set; }
        public bool Read { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
    }
}
