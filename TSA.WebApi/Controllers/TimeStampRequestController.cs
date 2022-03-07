using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Asn1.Tsp;
using Org.BouncyCastle.Tsp;
using Org.BouncyCastle.Utilities.Date;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using TSA.Infrastructure.Interfaces;
using TSA.WebApi.Filters;
using NLog;
using System.Text;
using Org.BouncyCastle.Math;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TSA.WebApi.Controllers
{
    [ServiceFilter(typeof(IpValidationFilter))]
    //[ServiceFilter(typeof(LoggerFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class TimeStampRequestController : ControllerBase
    {
        protected readonly ICertificateService _certificateService;
        protected readonly IProfileService _profileService;
        protected readonly IConfiguration _configuration;
        protected readonly IRequestLogService _requestLogService;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public TimeStampRequestController(ICertificateService certificateService, IProfileService profileService, 
            IConfiguration configuration, IRequestLogService requestLogService)
        {
            _certificateService = certificateService;
            _profileService = profileService;
            _configuration = configuration;
            _requestLogService = requestLogService;
        }


        // POST api/<ValuesController>
        [HttpPost]
        public async Task<FileStreamResult> Post()
        {
            System.Text.StringBuilder strLogg = new System.Text.StringBuilder();

            try {

                strLogg.AppendLine("receiving request.body");
                var requestStream = Request.Body;
                byte[] bytes;
                using (var memoryStream = new MemoryStream())
                {
                    await requestStream.CopyToAsync(memoryStream);
                    bytes = memoryStream.ToArray();
                }
                strLogg.AppendLine("converting to TimeStampRequest");
                TimeStampRequest tsr = new TimeStampRequest(bytes);

                X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                strLogg.AppendLine("openning X509Store");
                store.Open(OpenFlags.ReadOnly);

                X509Certificate2Collection certCollection = null;

                strLogg.AppendLine("getting all certificates from repository");
                var dbCertCollection = await _certificateService.GetAllCertificates();

                if (dbCertCollection.Count() > 0)
                    certCollection = store.Certificates.Find(X509FindType.FindByThumbprint, dbCertCollection.First().Thumbprint, false); // default cert to start proccess
                else
                    throw new Exception("No certificate ready");

                strLogg.AppendLine("initializating certificate");

                TSALibrary.Models.Certificate certificate;
                string reqPolicy;

                if (tsr.ReqPolicy == null)
                {
                    strLogg.AppendLine("Request Policy null, getting default certificarte");
                    certificate = await _certificateService.ReturnDefault();
                    reqPolicy = certificate.CertificatePolicies.FirstOrDefault().PolicyValue;
                    strLogg.AppendLine("Using default policy " + reqPolicy);
                }
                else
                {
                    strLogg.AppendLine("Request Policy "+ tsr.ReqPolicy + ", getting certificarte");
                    certificate = await _certificateService.ExistsCertWithPolicy(tsr.ReqPolicy);
                    reqPolicy = tsr.ReqPolicy;
                }
                if (certificate == null)
                    throw new Exception("No certificate has been found");


                TimeStampTokenGenerator tkngen = null;

                List<string> acceptedAlg = new List<string>();

                foreach (var p in (await _profileService.GetAllProfileValues()).Where(x => x.ProfileTypeId == 3))
                    acceptedAlg.Add(p.Value);

                List<string> acceptedPol = new List<string>();
                if (certificate != null)
                {
                    certCollection = store.Certificates.Find(X509FindType.FindByThumbprint, certificate.Thumbprint, false);
                    //acceptedPol.Add(tsr.ReqPolicy);
                    acceptedPol.Add(reqPolicy);


                }
                strLogg.AppendLine("initializating TimeStampTokenGenerator");

                if (certCollection.Count == 0) throw new Exception("Error initializing TimeStampToken");

                tkngen = new TimeStampTokenGenerator(null, new X509CertificateParser().ReadCertificate(certCollection[0].RawData), tsr.MessageImprintAlgOid, reqPolicy);

                strLogg.AppendLine("initializating TimeStampResponseGenerator");

                var resgen = new TimeStampResponseGenerator(tkngen, acceptedAlg, acceptedPol);

                strLogg.AppendLine("generating TimeStampResponse");

                var bigInteger = (tsr.Nonce==null) ? new BigInteger("1") : tsr.Nonce;

                var resp = resgen.Generate(tsr, bigInteger, new DateTimeObject(DateTime.UtcNow), certCollection[0].Thumbprint);//, _configuration.GetSection("SignerAPIUrl").Value);

                strLogg.AppendLine("encoding TimeStampResponse");

                var respBytes = resp.GetEncoded();

                strLogg.AppendLine("Log request/response");

                try {
                   int logId = (int)HttpContext.Items["logId"];
                    await SaveLog(resp, logId, "", reqPolicy, certificate.Id);
                }
                catch (Exception ex) 
                {
                    logger.Error("Error saving logRequest: " + ex.Message);
                }

                strLogg.AppendLine("returning TimeStampResponse");

                return File(new MemoryStream(respBytes), "application/timestamp");
            }
            catch (Exception e)
            {
                string error = e.Message;
                string inner = string.Empty;
                if (e.InnerException != null) {
                    inner = e.InnerException.Message;
                }
                System.Text.StringBuilder strFinalLogg = new System.Text.StringBuilder();
                strFinalLogg.AppendLine("Error Message: " + error);
                strFinalLogg.AppendLine("InnerException: " + inner);
                if (e.StackTrace != null)
                {
                    strFinalLogg.AppendLine("StackTrace: " + e.StackTrace);
                }
                strFinalLogg.AppendLine("Logger Lines Exception: ");
                strFinalLogg.AppendLine(strLogg.ToString());
                logger.Error(strFinalLogg.ToString());

                try { 
                    var statusStrings = new Asn1EncodableVector();
                     statusStrings.Add(new DerUtf8String("Certificate configuration error"));
                    var freeText = new PkiFreeText(new DerSequence(statusStrings));
                    var fail = new Org.BouncyCastle.Asn1.Cmp.PkiFailureInfo(Org.BouncyCastle.Asn1.Cmp.PkiFailureInfo.IncorrectData);
                    var reason = new Org.BouncyCastle.Asn1.Cmp.PkiStatusInfo((int)Org.BouncyCastle.Asn1.Cmp.PkiStatus.Rejection, freeText, fail);
                    var retVal = new TimeStampResp(reason, null);
                    var tsr = new TimeStampResponse(retVal);
                    var respBytesErr = tsr.GetEncoded();
                    try
                    {
                        int logId = (int)HttpContext.Items["logId"];
                        await SaveLog(tsr, logId, error, "", 0);
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Error saving logRequest: " + ex.Message);
                    }

                    return File(new MemoryStream(respBytesErr), "application/timestamp");
                }
                catch (Exception ee) {
                    var msgErr = "Error convertir catch error to TimeStampResponse" + ee.Message;
                    logger.Error(msgErr);
                    throw new Exception(msgErr);
                }
                throw e;
            }
        }


        private async Task SaveLog(TimeStampResponse resp, int logId, string message, string policy, int certificateId) {
            var req = await _requestLogService.GetRequestLogById(logId);
            var respBytesErr = resp.GetEncoded();
            var asn1node = PeNet.Asn1.Asn1Node.ReadNode(respBytesErr.ToArray()).ToXElement().ToString();
            req.Response = asn1node;
            req.ResponseStatusCode = resp.Status;
            req.ResponseStatusDescription = resp.GetStatusString();
            req.HttpStatusCode = HttpContext.Response.StatusCode;
            req.PolicyValue = policy;
            req.CertificateId = certificateId;
            if (message == "IPVALIDATORFILTER")
            {
                req.HttpErrorDescription = "Dirección IP no autorizada";
            }
            else if (message == "INVALIDUSER")
            {
                req.HttpErrorDescription = "Usuario no autorizado";
            }
            else if (message == "INVALIDPASSWORD")
            {
                req.HttpErrorDescription = "Password no concuerda";
            }
            else
            {
                req.HttpErrorDescription = message;
            }
            await _requestLogService.UpdateAndSave(req);
        }
     
    }
}