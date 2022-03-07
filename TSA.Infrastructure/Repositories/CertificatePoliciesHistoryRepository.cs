using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using TSA.Infrastructure.Data;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;

namespace TSA.Infrastructure.Repositories
{
    public class CertificatePoliciesHistoryRepository : GenericRepository<CertificatePoliciesHistory>, ICertificatePoliciesHistoryRepository
    {
        public CertificatePoliciesHistoryRepository(TSADbContext context) : base(context)
        {
        }
    }
}
