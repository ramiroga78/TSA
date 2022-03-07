using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using TSA.Infrastructure.Entities;
using TSALibrary.Models;

namespace TSA.Infrastructure.Interfaces
{
    public interface ICertificateOrganizationRepository : IGenericRepository<CertificateOrganization>
    {
        public Task<CertificateOrganization> GetCertificateOrganizationById(int id);

        public IEnumerable<SelectListItem> GetAllCertificateOrganizationByType(string type);
    }
}
