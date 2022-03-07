using System;
using System.Collections.Generic;
using TSA.Infrastructure.Entities;

namespace TSALibrary.Models 
{ 
    public partial class DeltaType : BaseEntity
    {
        public DeltaType()
        {
            Deltas = new HashSet<Delta>();
        }

        public int Id { get; set; }
        public string Description { get; set; }
        public int AddUserId { get; set; }
        public int? EditUserId { get; set; }
        public int? DeleteUserId { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime? EditDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<Delta> Deltas { get; set; }

    }
}
