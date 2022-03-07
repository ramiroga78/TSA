using System.Collections.Generic;
using System.Threading.Tasks;
using TSA.Infrastructure.Entities;
using TSALibrary.Models;

namespace TSA.Infrastructure.Interfaces
{
    public interface ICertificatePolicyService
    {
        public Task<IEnumerable<CertificatePolicy>> GetAllPolicies();

        public Task<CertificatePolicy> GetPolicyById(int id);

        public Task UpdateAndSave(CertificatePolicy policy);
              
        public Task<bool> AlgorithmExists(CertificatePolicy policy);

        public Task<int> AddAndSave(CertificatePolicy policy);
    }
}