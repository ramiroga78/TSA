using System;
using System.ComponentModel.DataAnnotations;

namespace TSA.Infrastructure.DTOs
{
    public class NTPServerDTO
    {
        public int Id { get; set; }
        public string ServerUrl { get; set; }
        public int AddUserId { get; set; }
        public int? EditUserId { get; set; }
        public int? DeleteUserId { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime? EditDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public bool IsActive { get; set; }
    }
}
