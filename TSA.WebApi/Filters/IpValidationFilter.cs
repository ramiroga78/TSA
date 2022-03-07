using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TSA.Infrastructure.Interfaces;
using TSA.Infrastructure.Services;

namespace TSA.WebApi.Filters
{
    public class IpValidationFilter : IAsyncActionFilter
    {
        private readonly IIpAddresService _ipAddresService;
        private readonly IExternalUserService _externalUserService;

        public IpValidationFilter(IIpAddresService ipAddressService, IExternalUserService externalUserService)
        {
            _ipAddresService = ipAddressService;
            _externalUserService = externalUserService;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string authErrorReason = await CheckAuthorizationAsync(context);
            if (authErrorReason == "OK")
            {
                await next();
            }
            else
            {
                var ip = context.HttpContext.Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                var list = await _ipAddresService.GetAllIpAddresses();
                bool valid = false;
                foreach (Infrastructure.DTOs.IpAddressDTO ipDTO in list)
                {
                    if (IsInRange(ipDTO.Name, ipDTO.Ip, ip))
                    {
                        valid = true;
                        break;
                    }
                }
                if (!valid)
                {
                    // context.Result = new UnauthorizedObjectResult("IPVALIDATORFILTER");
                    var result = new ObjectResult(new { statusCode = "402", currentDate = DateTime.Now });
                    result.StatusCode = 402;
                    context.Result = result;
                    return;
                }
                else
                {
                    if (authErrorReason != "NOUSERSPECIFIED")
                    {
                        context.Result = new UnauthorizedObjectResult(authErrorReason);
                        return;
                    }
                    else
                    {
                        await next();
                    }
                }
            }
        }
        public static bool IsInRange(string startIpAddr, string endIpAddr, string address)
        {
            long ipStart = BitConverter.ToInt32(IPAddress.Parse(startIpAddr).GetAddressBytes().Reverse().ToArray(), 0);

            long ipEnd = BitConverter.ToInt32(IPAddress.Parse(endIpAddr).GetAddressBytes().Reverse().ToArray(), 0);

            long ip = BitConverter.ToInt32(IPAddress.Parse(address).GetAddressBytes().Reverse().ToArray(), 0);

            return ip >= ipStart && ip <= ipEnd; //edited
        }

        public async Task<string> CheckAuthorizationAsync(ActionExecutingContext context)
        {
            IHeaderDictionary headers = context.HttpContext.Request.Headers;
            string retval = "NOUSERSPECIFIED";
            foreach (var pair in headers)
            {
                if (pair.Key == "Authorization")
                {
                    var credentials = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(string.Join(", ", pair.Value).Replace("Basic ", "")));
                    int separator = credentials.IndexOf(':');
                    string name = credentials.Substring(0, separator);
                    string password = credentials.Substring(separator + 1);
                    retval = await AuthenticateUser(name, password);

                    break;
                }
            }
            return retval;
        }

        private async Task<string> AuthenticateUser(string name, string password)
        {
            var user = await _externalUserService.GetUserByEmail(name);
            if (user != null)
            {
                bool passwordMatch = BCrypt.Net.BCrypt.Verify(password, user.Password);
                if (user.Email.ToLower() == user.Email.ToLower() && passwordMatch && user.IsActive == true)
                {
                    return "OK";
                }
                else
                {
                    return "INVALIDUSER";
                }
            }
            else
            {
                return "INVALIDUSER";
            }
        }
    }
}
