using System.Threading.Tasks;
using TSA.Infrastructure.Entities;
using TSALibrary.Models;

namespace TSA.Infrastructure.Interfaces
{
    public interface ICertificateRepository : IGenericRepository<Certificate>
    {
        public Task GetAllCertificates();
        public Task SoftDelete(int id);
        public Task<Certificate> GetCertificateByIdIncludingProfilesAndPolicies(int id);
    }
}
