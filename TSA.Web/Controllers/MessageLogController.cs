using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TSA.Infrastructure.DTOs;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;
using TSA.Web.Extensions;
using static TSA.Web.Models.DatatableModel;

namespace TSA.Web.Controllers
{
    public class MessageLogController : Controller
    {
        private readonly IMessageService _messageService;
        public MessageLogController(IMessageService messageService)
        {
            _messageService = messageService;
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
            var orderCriteria = "CreateDate";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                // in this example we just default sort on the 1st column
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            //var result = _context.CertificateOrganizations.AsQueryable();
            var result = _messageService.GetIQueryable();
            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(r => r.Subject != null && r.Subject.ToUpper().Contains(searchBy.ToUpper()));
            }
            //POR FECHA
            var fechaDesdeString = dtParameters.Columns[0].Search.Value;
            var fechaHastaString = dtParameters.Columns[1].Search.Value;
            DateTime fechaDesdeDateTime;
            DateTime fechaHastaDateTime;
            if (!string.IsNullOrEmpty(fechaDesdeString) && !string.IsNullOrEmpty(fechaHastaString))
            {
                fechaDesdeDateTime = Convert.ToDateTime(fechaDesdeString);
                fechaHastaDateTime = Convert.ToDateTime(fechaHastaString);
                result = result.Where(a => a.CreatedDate.Date >= fechaDesdeDateTime.Date && a.CreatedDate.Date <= fechaHastaDateTime.Date);
            }
            if (string.IsNullOrEmpty(fechaDesdeString) && !string.IsNullOrEmpty(fechaHastaString))
            {
                fechaHastaDateTime = Convert.ToDateTime(fechaHastaString);
                result = result.Where(a => a.CreatedDate.Date <= fechaHastaDateTime.Date);
            }
            if (!string.IsNullOrEmpty(fechaDesdeString) && string.IsNullOrEmpty(fechaHastaString))
            {
                fechaDesdeDateTime = Convert.ToDateTime(fechaDesdeString);
                result = result.Where(a => a.CreatedDate.Date >= fechaDesdeDateTime.Date);
            }
            //POR ESTADO DE ENVÍO
            string estadoString = dtParameters.Columns[2].Search.Value;
            if (!string.IsNullOrEmpty(estadoString))
            {
                if (estadoString == "Si")
                {
                    result = result.Where(a => a.Sent == true);
                }
                if (estadoString == "No")
                {
                    result = result.Where(a => a.Sent == false);
                }
            }
            //MOTIVO
            string subject = dtParameters.Columns[3].Search.Value;
            if (!string.IsNullOrEmpty(subject))
            {
                result = result.Where(a => a.Subject.Contains(subject));
            }
            //USUARIO
            string user = dtParameters.Columns[4].Search.Value;
            if (!string.IsNullOrEmpty(user))
            {
                result = result.Where(a => a.SentTo.Contains(user));
            }
            //SOLO LOS REQUEST DEL DÍA DE HOY (ESTE FILTRO SE HACE CUANDO NO SE INGRESA NINGÚN FILTRO)
            if (string.IsNullOrEmpty(fechaDesdeString) && string.IsNullOrEmpty(fechaHastaString) &&
                string.IsNullOrEmpty(estadoString) && string.IsNullOrEmpty(subject) &&
                string.IsNullOrEmpty(user))
                result = result.Where(x => x.CreatedDate.DayOfYear == DateTime.Now.DayOfYear);

            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);
            var filteredResultsCount = await result.CountAsync();
            var totalResultsCount = await _messageService.CountMessages();
            return Json(new DtResult<Message>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = await result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToListAsync()
            });
        }
    }
}
