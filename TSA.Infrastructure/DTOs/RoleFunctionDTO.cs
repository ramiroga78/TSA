using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSA.Infrastructure.DTOs
{
    public class RoleFunctionDTO
    {
        public string FunctionName { get; set; }
        public int FunctionId { get; set; }
        public int RoleId { get; set; }
        public bool Create { get; set; }
        public bool Read { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
    }
}
