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
    public class CertificateOrganizationHistoryRepository : GenericRepository<CertificateOrganizationHistory>, ICertificateOrganizationHistoryRepository
    {
        public CertificateOrganizationHistoryRepository(TSADbContext context) : base(context)
        {
        }
    }
}
