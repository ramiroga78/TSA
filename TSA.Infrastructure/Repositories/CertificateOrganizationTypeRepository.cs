using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSA.Infrastructure.Data;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;

namespace TSA.Infrastructure.Repositories
{
    public class CertificateOrganizationTypeRepository : GenericRepository<CertificateOrganizationType>, ICertificateOrganizationTypeRepository
    {
        public CertificateOrganizationTypeRepository(TSADbContext context) : base(context)
        {
        }

        public async Task<CertificateOrganizationType> GetCertificateOrganizationTypeById (int id)
        {
            var certificateOrganizationType = await _entities.FindAsync(id);
            return certificateOrganizationType;
        }

    }
}
