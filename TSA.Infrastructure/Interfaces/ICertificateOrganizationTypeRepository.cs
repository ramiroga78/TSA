using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSA.Infrastructure.Entities;
using TSALibrary.Models;

namespace TSA.Infrastructure.Interfaces
{
    public interface ICertificateOrganizationTypeRepository : IGenericRepository<CertificateOrganizationType>
    {
        public Task<CertificateOrganizationType> GetCertificateOrganizationTypeById(int id);
    }
}
