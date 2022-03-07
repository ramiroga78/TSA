using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSA.Utilities
{
    public class Response
    {
        public class ResponseValues
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }
        public string Result { get; set; }
        public string Message { get; set; }
        public virtual ICollection<ResponseValues> Values{ get; set; }
        public Response()
        {
            Values = new List<Response.ResponseValues>();
        }
    }
}
