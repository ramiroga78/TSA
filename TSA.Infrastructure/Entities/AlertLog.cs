using System;
using TSALibrary.Models;

namespace TSA.Infrastructure.Entities
{
    public class AlertLog : BaseEntity
    {
        public DateTime DateFired { get; set; }
        public int AlertId { get; set; }
        public Alert Alert { get; set; }
    }
}
