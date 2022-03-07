using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSA.Infrastructure.Entities;

namespace TSALibrary.Models
{
    public partial class Message : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public int IdType { get; set; }
        public int? IdDelta { get; set; }
        public int? IdAlerta { get; set; }
        public int IdUser { get; set; }
        public string SentTo { get; set; }
        public string Subject { get; set; }
        public string MessageBody { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Sent { get; set; }
        public DateTime? SentDate { get; set; }
        public bool SentError { get; set; }
        public string ErrorReason { get; set; }
        public DateTime? EditDate { get; set; }
    }
}
