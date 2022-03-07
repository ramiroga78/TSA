using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Tsp;
using PeNet.Asn1;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;

namespace TSA.Infrastructure.DependencyInjection
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public RequestResponseLoggingMiddleware(RequestDelegate next,
                                                IServiceProvider ServiceProvider,
                                                IConfiguration configuration)
        {
            _next = next;
            _serviceProvider = ServiceProvider;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            await LogRequest(context);
            //await LogResponse(context);
            //await _next(context);
        }

        private async Task LogRequest(HttpContext context)
        {
            if (context.Request.Path.Equals("/api/TimeStampRequest"))
            {
                using (var scope = _serviceProvider.CreateScope())
                using (var streamCopy = new MemoryStream())
                using (var swapStream = new MemoryStream())
                {
                    var format = _configuration.GetValue<string>("ResponseLogFormat");
                    var requestLogService = scope.ServiceProvider.GetRequiredService<IRequestLogService>();// get response
                    var certificateService = scope.ServiceProvider.GetRequiredService<ICertificateService>();
                    await context.Request.Body.CopyToAsync(streamCopy);
                    var requestLog = new RequestLog();
                    try
                    {
                        var asn1node = Asn1Node.ReadNode(streamCopy.ToArray()).ToXElement().ToString();//get ans1 xml representation string
                        streamCopy.Position = 0;
                        TimeStampRequest tsr = new TimeStampRequest(streamCopy.ToArray());
                        streamCopy.Position = 0;
                        string body = new StreamReader(streamCopy).ReadToEnd();
                        if (format.Equals("xml"))
                            requestLog.Request = asn1node;
                        else
                        {
                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(asn1node);
                            requestLog.Request = JsonConvert.SerializeXmlNode(doc);
                        }
                        requestLog.ClientIp = context.Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                        requestLog.ExternalUser = GetExternalUser(context);
                        requestLog.PolicyValue = tsr.ReqPolicy;
                    }
                    catch (Exception e)
                    {
                        requestLog.HttpErrorDescription = e.Message;
                        try { context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; } catch { }
                    }
                    finally
                    {
                        await requestLogService.AddAndSave(requestLog);
                        streamCopy.Position = 0;
                        context.Request.Body = streamCopy;// replace stream to continue working
                        var originalResponseBody = context.Response.Body;//replace response stream to enable writing and reading after signing
                        //context.Response.Body = swapStream; // Esta linea la comento, esta reiniciando en content-length del response y da error al enviarlo.

                        var req = await requestLogService.GetRequestLogById(requestLog.Id);
                        context.Items["logId"] = requestLog.Id;
                         //await _next(context);// continue up in the pipeline
                        try
                        {
                            await _next(context);// continue up in the pipeline
                            swapStream.Seek(0, SeekOrigin.Begin);
                            if (context.Response.StatusCode != 200)
                            {
                                //var streamReader = new System.IO.StreamReader(swapStream, System.Text.Encoding.UTF8);
                                var streamReader = new System.IO.StreamReader(context.Response.Body, System.Text.Encoding.UTF8); 
                                swapStream.Seek(0, SeekOrigin.Begin);
                                throw new Exception(await streamReader.ReadToEndAsync());
                            }

                            //string responseBody = new StreamReader(swapStream).ReadToEnd();
                            //swapStream.Seek(0, SeekOrigin.Begin);
                            //await swapStream.CopyToAsync(originalResponseBody);
                            //swapStream.Seek(0, SeekOrigin.Begin);

                            //var asn1Stream = new Asn1InputStream(swapStream.ToArray());
                            //var asn1node = string.Empty;

                            //try { asn1node = Asn1Node.ReadNode(swapStream.ToArray()).ToXElement().ToString(); }
                            //catch (Exception ee)
                            //{

                            //}

                            //if (format.Equals("xml"))
                            //    requestLog.Response = asn1node;
                            //else
                            //{
                            //    XmlDocument doc = new XmlDocument();
                            //    doc.LoadXml(asn1node);
                            //    requestLog.Response = JsonConvert.SerializeXmlNode(doc);
                            //}

                            //var rsp = new TimeStampResponse(asn1Stream);
                            //if (rsp.Status != 2)// policy not found
                            //    if (requestLog.PolicyValue != null)
                            //    {
                            //        requestLog.CertificateId = (await certificateService.ExistsCertWithPolicy(requestLog.PolicyValue)).Id;
                            //    }
                            //requestLog.ResponseStatusCode = rsp.Status;
                            //requestLog.ResponseStatusDescription = rsp.GetStatusString();
                        }
                        catch (Exception e)
                        {

                            if (context.Response.StatusCode == 402) {
                                requestLog.HttpErrorDescription = "Dirección IP no autorizada";
                                try { context.Response.StatusCode = (int)HttpStatusCode.Unauthorized; } catch { }
                            }
                            else if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                            {
                                requestLog.HttpErrorDescription = "Usuario no autorizado";
                            }
                            else
                            {
                                requestLog.HttpErrorDescription = e.Message;
                                try { context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; } catch { }
                            }
                            requestLog.HttpStatusCode = context.Response.StatusCode;
                            await requestLogService.UpdateAndSave(requestLog);
                        }
                        finally
                        {

                            //context.Response.Body = originalResponseBody;
                        }
                    }
                }
            }
            else
                await _next(context);
        }


        private string GetExternalUser(HttpContext context) {
            IHeaderDictionary headers = context.Request.Headers;
            string retval = string.Empty;
            foreach (var pair in headers)
            {
                if (pair.Key == "Authorization")
                {
                    var credentials = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(string.Join(", ", pair.Value).Replace("Basic ", "")));
                    int separator = credentials.IndexOf(':');
                    string name = credentials.Substring(0, separator);
                    retval = name;
                    break;
                }
            }
            return retval;
        }
    }
}