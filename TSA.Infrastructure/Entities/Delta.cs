using System;
using System.Collections.Generic;
using TSA.Infrastructure.Entities;

#nullable disable

namespace TSALibrary.Models
{
    public partial class Delta : BaseEntity
    {
        public Delta()
        {
            DeltaUsers = new HashSet<DeltaUser>();
        }

        public int Id { get; set; }
        public int DeltaTypeId { get; set; }
        public string EventCode { get; set; }
        public string EventName { get; set; }
        public string EventDescription { get; set; }
        public string ControlName { get; set; }
        public string ControlOperator { get; set; }
        public string ControlOperatorValue { get; set; }
        public bool StopService { get; set; }
        public int AddUserId { get; set; }
        public int? EditUserId { get; set; }
        public int? DeleteUserId { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime? EditDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public bool IsActive { get; set; }

        public virtual DeltaType DeltaTypes { get; set; }
        public virtual ICollection<DeltaUser> DeltaUsers { get; set; }
    }
}