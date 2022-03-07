using System;
using TSA.Infrastructure.Entities;

#nullable disable

namespace TSALibrary.Models
{
    public partial class RequestLog : BaseEntity
    {
        public int Id { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime ResponseDate { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string ClientIp { get; set; }
        public int CertificateId { get; set; }
        public int HttpStatusCode { get; set; }
        public int ResponseStatusCode { get; set; }
        public string HttpErrorDescription{ get; set; }
        public string ResponseStatusDescription { get; set; }
        public string PolicyValue { get; set; }
        public string ExternalUser { get; set; }

    }
}