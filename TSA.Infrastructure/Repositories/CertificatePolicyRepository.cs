using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using TSA.Infrastructure.Data;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;

namespace TSA.Infrastructure.Repositories
{
    public class CertificatePolicyRepository : GenericRepository<CertificatePolicy>, ICertificatePolicyRepository
    {
        public CertificatePolicyRepository(TSADbContext context) : base(context)
        {
        }

        public Task<bool> AlgorithmItemExists(CertificatePolicy algorithm)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CertificatePolicyExists(CertificatePolicy policy)
        {
            return await _entities.AnyAsync(x => x.PolicyValue.Equals(policy.PolicyValue, StringComparison.OrdinalIgnoreCase));
        }
    }
}