using System;
using System.Collections.Generic;

#nullable disable

namespace TSALibrary.Models
{
    public partial class AlertsLog
    {
        public int Id { get; set; }
        public DateTime DateFired { get; set; }
        public int AlertId { get; set; }

        public virtual Alert Alert { get; set; }
    }
}
