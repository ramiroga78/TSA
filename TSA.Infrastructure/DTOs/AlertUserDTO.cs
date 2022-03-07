using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSA.Infrastructure.DTOs
{
    public class AlertUserDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int AlertId { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}
