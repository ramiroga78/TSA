using System;
using TSA.Infrastructure.Entities;

#nullable disable

namespace TSALibrary.Models
{
    public partial class DeltaUser : BaseEntity
    {

        public int Id { get; set; }
        public int DeltaId { get; set; }
        public int UserId { get; set; }
        public int AddUserId { get; set; }
        public int? EditUserId { get; set; }
        public int? DeleteUserId { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime? EditDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public bool IsActive { get; set; }
        public virtual Delta Deltas { get; set; }
        public virtual User Users { get; set; }
    }
}
