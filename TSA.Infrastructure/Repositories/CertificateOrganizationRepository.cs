using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TSA.Infrastructure.Data;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;

namespace TSA.Infrastructure.Repositories
{
    public class CertificateOrganizationRepository : GenericRepository<CertificateOrganization>, ICertificateOrganizationRepository
    {
        private readonly TSADbContext _context;
        public CertificateOrganizationRepository(TSADbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<CertificateOrganization> GetCertificateOrganizationById(int id)
        {
            var certificateOrganization = await _entities.FindAsync(id);
            return certificateOrganization;
        }

        public IEnumerable<SelectListItem> GetAllCertificateOrganizationByType(string type)
        {
            var certOrgType = _context.CertificateOrganizationTypes.Where(x => x.Description == type).FirstOrDefault();

            return _context.CertificateOrganizations
            .Where(i => i.CertificateOrganizationTypeId == certOrgType.Id)
            .Select(i => new SelectListItem()
            {
                Text = i.CommonName,
                Value = i.Id.ToString()
            });
        }
    }
}
