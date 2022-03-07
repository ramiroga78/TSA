using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSA.Infrastructure.DTOs
{
    public class DeltaDTO
    {
        public int Id { get; set; }
        public int DeltaTypeId { get; set; }
        public string EventCode { get; set; }
        public string EventName { get; set; }
        public string EventDescription { get; set; }
        public bool StopService { get; set; }
        public string ControlName { get; set; }
        public string ControlOperator { get; set; }
        public string ControlOperatorValue { get; set; }
        public int UserCount { get; set; }
        public bool? IsActive { get; set; }
    }
}
