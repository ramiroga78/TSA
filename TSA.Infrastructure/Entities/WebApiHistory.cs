using System;

namespace TSA.Infrastructure.Entities
{
    public class WebApiHistory : BaseEntity
    {
        //Creamos un enum con tipo Request y Response?
        //public RequestTypeEnum RequestType { get; set; }
        public string Data { get; set; }
        public DateTime Datetime { get; set; }
    }
}
