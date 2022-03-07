using System.ComponentModel.DataAnnotations;
using System.Net;

namespace TSA.Web.CustomValidation
{
    public class IpValidator: ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            IPAddress ip;
            return IPAddress.TryParse(value.ToString(), out ip);
        }
    }
}
