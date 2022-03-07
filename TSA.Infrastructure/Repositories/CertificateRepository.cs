using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TSA.Infrastructure.Data;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;

namespace TSA.Infrastructure.Repositories
{
    public class CertificateRepository : GenericRepository<Certificate>, ICertificateRepository
    {
        private readonly TSADbContext _context;

        public CertificateRepository(TSADbContext context) : base(context)
        {
            _context = context;
        }

        public Task GetAllCertificates()
        {
            throw new System.NotImplementedException();
        }

        public async Task SoftDelete(int id)
        {
            //throw new System.NotImplementedException();
            var certificate = await _entities.FindAsync(id);
            certificate.IsActive = false;
        }

        public Task<Certificate> GetCertificateByIdIncludingProfilesAndPolicies(int id)
        {
            return _entities
                .Where(cert => cert.Id == id)
                .Include(cert => cert.CertificateProfiles)
                    .ThenInclude(certprof => certprof.ProfileType)
                .Include(cert => cert.CertificateProfiles)
                    .ThenInclude(profval => profval.ProfileValue)
                .Include(cert => cert.CertificatePolicies)
                    .ThenInclude(pol => pol.PolicyType)
                .FirstOrDefaultAsync();
        }

    }
}
