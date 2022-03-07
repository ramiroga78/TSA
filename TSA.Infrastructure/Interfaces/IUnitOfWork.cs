using System;
using System.Threading.Tasks;

namespace TSA.Infrastructure.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        public Task<int> SaveChangesAsync();

        //Add Interfaces
        IUserRepository UserRepository { get; }
        IUserHistoryRepository UserHistoryRepository { get; }

        IExternalUserRepository ExternalUserRepository { get; }
        IExternalUserHistoryRepository ExternalUserHistoryRepository { get; }

        IRoleRepository RoleRepository { get; }
        IProfileTypeRepository ProfileTypeRepository { get; }
        IProfileTypeHistoryRepository ProfileTypeHistoryRepository { get; }
        IProfileValueRepository ProfileValueRepository { get; }
        IProfileValueHistoryRepository ProfileValueHistoryRepository { get; }
        IIpAddressRepository IpAddressRepository { get; }
        IIpAddressHistoryRepository IpAddressHistoryRepository { get; }
        IRoleHistoryRepository RoleHistoryRepository { get; }
        IFunctionRepository FunctionRepository { get; }
        IRoleFunctionRepository RoleFunctionRepository { get; }
        IRoleUserRepository RoleUserRepository { get; }
        IRoleUserHistoryRepository RoleUserHistoryRepository { get; }
        IRequestLogRepository RequestLogRepository { get; }
        INTPServerRepository NTPServerRepository { get; }
        INTPServerHistoryRepository NTPServerHistoryRepository { get; }       
        ICertificatePolicyRepository CertificatePolicyRepository { get; }
        ICertificateOrganizationRepository CertificateOrganizationRepository { get; }
        ICertificateOrganizationHistoryRepository CertificateOrganizationHistoryRepository { get; }
        ICertificateOrganizationTypeRepository CertificateOrganizationTypeRepository { get; }
        IRoleFunctionHistoryRepository RoleFunctionHistoryRepository { get; }
        IPolicyTypeHistoryRepository PolicyTypeHistoryRepository{ get; }
        IPolicyTypeRepository PolicyTypeRepository{ get; }
        IAlertRepository AlertRepository { get; }
        ICertificateRepository CertificateRepository { get; }
        IAlertUserRepository AlertUserRepository { get; }
        IAlertUserHistoryRepository AlertUserHistoryRepository { get; }
        ICertificatesHistoryRepository CertificatesHistoryRepository { get; }
        ICertificateProfilesHistoryRepository CertificateProfilesHistoryRepository { get; }
        ICertificatePoliciesHistoryRepository CertificatePoliciesHistoryRepository { get; }
        ICertificateProfileRepository CertificateProfileRepository { get; }
        IMessageRepository MessageRepository { get; }
        IDeltaRepository DeltaRepository { get; }
        IDeltaHistoryRepository DeltaHistoryRepository { get; }
        IDeltaUserRepository DeltaUserRepository { get; }
        IDeltaUserHistoryRepository DeltaUserHistoryRepository { get;  }
        IDeltaTypeRepository DeltaTypeRepository { get; }
        IDeltaTypeHistoryRepository DeltaTypeHistoryRepository { get; }

        public Task BeginTransactionAsync();

        public Task CommitAsync();

        public Task RollBackAsync();
    }
}