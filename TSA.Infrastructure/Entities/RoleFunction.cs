using System;
using System.Collections.Generic;
using TSA.Infrastructure.Entities;

#nullable disable

namespace TSALibrary.Models
{
    public partial class RoleFunction : BaseEntity
    {
        public int FunctionId { get; set; }
        public int RoleId { get; set; }
        public int Id { get; set; }
        public int AddUserId { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime EditDate { get; set; }
        public bool Create { get; set; }
        public bool Read { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }

        public virtual Function Function { get; set; }
        public virtual Role Role { get; set; }

        public static implicit operator List<object>(RoleFunction v)
        {
            throw new NotImplementedException();
        }
    }
}
