using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSA.Infrastructure.DTOs
{
    public class MessageLogDTO
    {
        public DateTime CreatedDate { get; set; }
        public DateTime? SentDate { get; set; }
        public string Sent { get; set; }
        public string Subject { get; set; }
        public string User { get; set; }

    }
}
