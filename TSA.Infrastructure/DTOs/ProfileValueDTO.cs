using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSA.Infrastructure.DTOs
{
    public class ProfileValueDTO
    {
        public int Id { get; set; }
        public int ProfileTypeId { get; set; }
        public string Value { get; set; }
        public bool IsActive { get; set; }
    }
}
