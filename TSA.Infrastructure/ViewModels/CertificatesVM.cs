using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TSALibrary.Models;

namespace TSA.Infrastructure.ViewModels
{
    public class CertificatesVM
    {
        public Certificate Certificate{ get; set; }
        public string CommonName { get; set; }
        public IEnumerable<SelectListItem> IssuersList { get; set; }
        public IEnumerable<SelectListItem> SubjectsList { get; set; }
        public IEnumerable<SelectListItem> ProfilesTypesList { get; set; }
        public IEnumerable<SelectListItem> ProfilesValuesList { get; set; }
        public IEnumerable<SelectListItem> PoliciesTypesList { get; set; }
    }
}
