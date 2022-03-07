using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TSA.Infrastructure.Interfaces;
using TSA.Web.Extensions;
using TSALibrary.Models;
using static TSA.Web.Models.DatatableModel;

namespace TSA.Web.Controllers
{
    public class RequestLogController : Controller
    {
        private readonly IRequestLogService _requestLogService;
        private readonly ICertificatePolicyService _certificatePolicyService;
        public RequestLogController(IRequestLogService requestLogService, ICertificatePolicyService certificatePolicyService)
        {
            _requestLogService = requestLogService;
            _certificatePolicyService = certificatePolicyService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoadTable([FromBody] DtParameters dtParameters)
        {
            var searchBy = dtParameters.Search?.Value;            
            // if we have an empty search then just order the results by Id ascending
            var orderCriteria = "Id";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                // in this example we just default sort on the 1st column
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            //var result = _context.CertificateOrganizations.AsQueryable();
            var result = _requestLogService.GetIQueryable();
            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(r => r.ResponseStatusDescription != null && r.ResponseStatusDescription.ToUpper().Contains(searchBy.ToUpper()) ||
                                           r.HttpErrorDescription != null && r.HttpErrorDescription.ToUpper().Contains(searchBy.ToUpper()));
            }
            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            //FILTROS            
            //FILTRO POR ESTADO DEL RESPONSE
            string responseStatus = dtParameters.Columns[0].Search.Value;
            if (!string.IsNullOrEmpty(responseStatus))
            {
                result = result.Where(a => a.ResponseStatusDescription.Contains(responseStatus));
            }
            //POR FECHA
            var fechaDesdeString = dtParameters.Columns[1].Search.Value;
            var fechaHastaString = dtParameters.Columns[2].Search.Value;
            DateTime fechaDesdeDateTime; 
            DateTime fechaHastaDateTime;
            if (!string.IsNullOrEmpty(fechaDesdeString) && !string.IsNullOrEmpty(fechaHastaString))
            {
                fechaDesdeDateTime = Convert.ToDateTime(fechaDesdeString);
                fechaHastaDateTime = Convert.ToDateTime(fechaHastaString);
                result = result.Where(a => a.RequestDate.Date >= fechaDesdeDateTime.Date && a.RequestDate.Date <= fechaHastaDateTime.Date);
            }
            if (string.IsNullOrEmpty(fechaDesdeString) && !string.IsNullOrEmpty(fechaHastaString))
            {
                fechaHastaDateTime = Convert.ToDateTime(fechaHastaString);
                result = result.Where(a => a.RequestDate.Date <= fechaHastaDateTime.Date);
            }
            if (!string.IsNullOrEmpty(fechaDesdeString) && string.IsNullOrEmpty(fechaHastaString))
            {
                fechaDesdeDateTime = Convert.ToDateTime(fechaDesdeString);
                result = result.Where(a => a.RequestDate.Date >= fechaDesdeDateTime.Date);
            }
            //FILTRO POR POLICYID
            string policyValue = dtParameters.Columns[3].Search.Value;
            if (!string.IsNullOrEmpty(policyValue))
            {
                result = result.Where(a => a.PolicyValue.Contains(policyValue));
            }
            //FILTRO POR CÓDIGO DE ERROR
            string errorCode = dtParameters.Columns[6].Search.Value;
            if (!string.IsNullOrEmpty(errorCode))
            {
                result = result.Where(a => a.HttpErrorDescription.Contains(errorCode));
            }
            //SOLO LOS REQUEST DEL DÍA DE HOY (ESTE FILTRO SE HACE CUANDO NO SE INGRESA NINGÚN FILTRO)
            if (string.IsNullOrEmpty(fechaDesdeString) && string.IsNullOrEmpty(fechaHastaString) &&
                string.IsNullOrEmpty(responseStatus) && string.IsNullOrEmpty(policyValue) &&
                string.IsNullOrEmpty(errorCode))
                result = result.Where(x => x.ResponseDate.DayOfYear == DateTime.Now.DayOfYear);
            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            var filteredResultsCount = await result.CountAsync();
            var totalResultsCount = await _requestLogService.CountRequestLog();
            var resultData = await result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToListAsync();

            return Json(new DtResult<RequestLog>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = resultData
            });
        }

        [HttpGet]
        [Produces("application/xml")]
        public async Task<IActionResult> Request(int id)
        {
            var requestRespondeDto = await _requestLogService.GetRequest(id);
            return new ContentResult
            {
                ContentType = "application/xml",
                Content = requestRespondeDto,
                StatusCode = 200
            };
        }
        [HttpGet]
        [Produces("application/xml")]
        public async Task<IActionResult> Response(int id)
        {
            var requestRespondeDto = await _requestLogService.GetResponse(id);
            return new ContentResult
            {
                ContentType = "application/xml",
                Content = requestRespondeDto,
                StatusCode = 200
            };
        }
    }
}
