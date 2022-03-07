using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;

namespace TSA.Infrastructure.Services
{
    public class CertificatePolicyService : BaseService, ICertificatePolicyService
    {
        public CertificatePolicyService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public async Task<IEnumerable<CertificatePolicy>> GetAllPolicies()
        {
            try
            {
                var policies = await _unitOfWork.CertificatePolicyRepository.GetAllAsync();

                return policies;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<CertificatePolicy> GetPolicyById(int id)
        {
            try
            {
                var policy = await _unitOfWork.CertificatePolicyRepository.GetByIdAsync(id);

                return policy;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<int> AddAndSave(CertificatePolicy policy)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _unitOfWork.CertificatePolicyRepository.Insert(policy);
                //await _unitOfWork.UserHistoryRepository.Insert(userHistory);

                int result = await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitAsync();

                return result;
            }
            catch (System.Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        public async Task<bool> AlgorithmExists(CertificatePolicy policy)
        {
            //to do revesiar
            return false;
        }

        //Keep in mind that this method updates every property of the User, also those that aren't modified or aren't specified (nulling them)
        public async Task UpdateAndSave(CertificatePolicy algorithm)
        {
            try
            {
                // var algorithmEntity = _mapper.Map<Algorithm>(algorithm);

                _unitOfWork.CertificatePolicyRepository.Update(algorithm);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}