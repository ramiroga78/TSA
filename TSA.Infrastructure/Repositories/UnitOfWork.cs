using System;
using System.Threading.Tasks;
using TSA.Infrastructure.Data;
using TSA.Infrastructure.Interfaces;

namespace TSA.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TSADbContext _context;
        private bool _disposed = false;

        //Add Repositoriesd
        private UserRepository _userRepository;
        private UserHistoryRepository _userHistoryRepository;
        private ExternalUserRepository _externalUserRepository;
        private ExternalUserHistoryRepository _externalUserHistoryRepository;

        private RoleRepository _roleRepository;
        private ProfileValueRepository _profileValueRepository;
        private ProfileValueHistoryRepository _profileValueHistoryRepository;
        private ProfileTypeRepository _profileTypeRepository;
        private ProfileTypeHistoryRepository _profileTypeHistoryRepository;
        private RoleHistoryRepository _roleHistoryRepository;
        private IpAddressRepository _ipAddressRepository;
        private IpAddressHistoryRepository _ipAddressHistoryRepository;
        private FunctionRepository _functionRepository;
        private RoleFunctionRepository _roleFunctionRepository;
        private RoleUserRepository _roleUserRepository;
        private RoleUserHistoryRepository _roleUserHistoryRepository;
        private RequestLogRepository _requestLogRepository;
        private NTPServerRepository _ntpServerRepository;
        private NTPServerHistoryRepository _ntpServerHistoryRepository;
        private CertificateOrganizationRepository _certificateOrganizationRepository;
        private ICertificatePolicyRepository _certificatePolicyRepository;
        private CertificateOrganizationTypeRepository _certificateOrganizationTypeRepository;
        private CertificateOrganizationHistoryRepository _certificateOrganizationHistoryRepository;
        private CertificateRepository _certificateRepository;
        private RoleFunctionHistoryRepository _roleFunctionHistoryRepository;
        private PolicyTypeRepository _policyTypeRepository;
        private PolicyTypeHistoryRepository _policyTypeHistoryRepository;
        private CertificatesHistoryRepository _certificatesHistoryRepository;
        private CertificateProfilesHistoryRepository _certificateProfilesHistoryRepository;
        private CertificatePoliciesHistoryRepository _certificatePoliciesHistoryRepository;
        private CertificateProfileRepository _certificateProfileRepository;
        private AlertRepository _alertRepository;
        private AlertUserRepository _alertUserRepository;
        private AlertUserHistoryRepository _alertUserHistoryRepository;
        private MessageRepository _messageRepository;
        private DeltaRepository _deltaRepository;
        private DeltaHistoryRepository _deltaHistoryRepository;
        private DeltaUserRepository _deltaUserRepository;
        private DeltaUserHistoryRepository _deltaUserHistoryRepository;
        private DeltaTypeRepository _deltaTypeRepository;
        private DeltaTypeHistoryRepository _deltaTypeHistoryRepository;

        public UnitOfWork(TSADbContext context)
        {
            _context = context;
        }

        //Add Interfaces
        public IUserRepository UserRepository
        {
            get
            {
                if (_userRepository == null)
                {
                    _userRepository = new UserRepository(_context);
                    return _userRepository;
                }

                return _userRepository;
            }
        }

        public IUserHistoryRepository UserHistoryRepository
        {
            get
            {
                if (_userHistoryRepository == null)
                {
                    _userHistoryRepository = new UserHistoryRepository(_context);
                    return _userHistoryRepository;
                }

                return _userHistoryRepository;
            }
        }

        //Add Interfaces
        public IExternalUserRepository ExternalUserRepository
        {
            get
            {
                if (_externalUserRepository == null)
                {
                    _externalUserRepository = new ExternalUserRepository(_context);
                    return _externalUserRepository;
                }

                return _externalUserRepository;
            }
        }

        public IExternalUserHistoryRepository ExternalUserHistoryRepository
        {
            get
            {
                if (_externalUserHistoryRepository == null)
                {
                    _externalUserHistoryRepository = new ExternalUserHistoryRepository(_context);
                    return _externalUserHistoryRepository;
                }

                return _externalUserHistoryRepository;
            }
        }

        public IRoleRepository RoleRepository
        {
            get
            {
                if (_roleRepository == null)
                {
                    _roleRepository = new RoleRepository(_context);
                    return _roleRepository;
                }

                return _roleRepository;
            }
        }
        public IProfileTypeRepository ProfileTypeRepository
        {
            get
            {
                if(_profileTypeRepository == null)
                {
                    _profileTypeRepository = new ProfileTypeRepository(_context);
                    return _profileTypeRepository;
                }
                return _profileTypeRepository;
            }
        }
        public IProfileTypeHistoryRepository ProfileTypeHistoryRepository
        {
            get
            {
                if(_profileTypeHistoryRepository == null)
                {
                    _profileTypeHistoryRepository = new ProfileTypeHistoryRepository(_context);
                    return _profileTypeHistoryRepository;
                }
                return _profileTypeHistoryRepository;
            }
        }
        public IProfileValueRepository ProfileValueRepository
        {
            get
            {
                if(_profileValueRepository == null)
                {
                    _profileValueRepository = new ProfileValueRepository(_context);
                    return _profileValueRepository;
                }
                return _profileValueRepository;
            }
        }
        public IProfileValueHistoryRepository ProfileValueHistoryRepository
        {
            get
            {
                if(_profileValueHistoryRepository == null)
                {
                    _profileValueHistoryRepository = new ProfileValueHistoryRepository(_context);
                    return _profileValueHistoryRepository;
                }
                return _profileValueHistoryRepository;
            }
        }
        
        public IIpAddressRepository IpAddressRepository
        {
            get
            {
                if (_ipAddressRepository == null)
                {
                    _ipAddressRepository = new IpAddressRepository(_context);
                    return _ipAddressRepository;
                }
                return _ipAddressRepository;
            }
        }

        public IIpAddressHistoryRepository IpAddressHistoryRepository
        {
            get
            {
                if (_ipAddressHistoryRepository == null)
                {
                    _ipAddressHistoryRepository = new IpAddressHistoryRepository(_context);
                    return _ipAddressHistoryRepository;
                }

                return _ipAddressHistoryRepository;
            }
        }

        public IRoleHistoryRepository RoleHistoryRepository
        {
            get
            {
                if (_roleHistoryRepository == null)
                {
                    _roleHistoryRepository = new RoleHistoryRepository(_context);
                    return _roleHistoryRepository;
                }

                return _roleHistoryRepository;
            }
        }

        public IFunctionRepository FunctionRepository
        {
            get
            {
                if (_functionRepository == null)
                {
                    _functionRepository = new FunctionRepository(_context);
                    return _functionRepository;
                }
                return _functionRepository;
            }
        }

        public IRoleFunctionRepository RoleFunctionRepository
        {
            get
            {
                if (_roleFunctionRepository == null)
                {
                    _roleFunctionRepository = new RoleFunctionRepository(_context);
                    return _roleFunctionRepository;
                }
                return _roleFunctionRepository;
            }
        }



        public IRoleUserRepository RoleUserRepository
        {
            get
            {
                if (_roleUserRepository == null)
                {
                    _roleUserRepository = new RoleUserRepository(_context);
                    return _roleUserRepository;
                }

                return _roleUserRepository;
            }
        }

        public IRoleUserHistoryRepository RoleUserHistoryRepository
        {
            get
            {
                if (_roleUserHistoryRepository == null)
                {
                    _roleUserHistoryRepository = new RoleUserHistoryRepository(_context);
                    return _roleUserHistoryRepository;
                }

                return _roleUserHistoryRepository;
            }
        }
        public ICertificateOrganizationTypeRepository CertificateOrganizationTypeRepository
        {
            get
            {
                if (_certificateOrganizationTypeRepository == null)
                {
                    _certificateOrganizationTypeRepository = new CertificateOrganizationTypeRepository(_context);
                    return _certificateOrganizationTypeRepository;
                }

                return _certificateOrganizationTypeRepository;
            }
        }
        public ICertificateOrganizationHistoryRepository CertificateOrganizationHistoryRepository
        {
            get
            {
                if (_certificateOrganizationHistoryRepository == null)
                {
                    _certificateOrganizationHistoryRepository = new CertificateOrganizationHistoryRepository(_context);
                    return _certificateOrganizationHistoryRepository;
                }

                return _certificateOrganizationHistoryRepository;
            }
        }

        public ICertificateOrganizationRepository CertificateOrganizationRepository
        {
            get
            {
                if (_certificateOrganizationRepository == null)
                {
                    _certificateOrganizationRepository = new CertificateOrganizationRepository(_context);
                    return _certificateOrganizationRepository;
                }

                return _certificateOrganizationRepository;
            }
        }

        public IRequestLogRepository RequestLogRepository
        {
            get
            {
                if (_requestLogRepository == null)
                {
                    _requestLogRepository = new RequestLogRepository(_context);
                    return _requestLogRepository;
                }

                return _requestLogRepository;
            }
        }

        public IRoleFunctionHistoryRepository RoleFunctionHistoryRepository
        {
            get
            {
                if (_roleFunctionHistoryRepository == null)
                {
                    _roleFunctionHistoryRepository = new RoleFunctionHistoryRepository(_context);
                    return _roleFunctionHistoryRepository;
                }

                return _roleFunctionHistoryRepository;
            }
        }

        public IPolicyTypeRepository PolicyTypeRepository
        {
            get
            {
                if (_policyTypeRepository == null)
                {
                    _policyTypeRepository = new PolicyTypeRepository(_context);
                    return _policyTypeRepository;
                }

                return _policyTypeRepository;
            }
        }

        public IPolicyTypeHistoryRepository PolicyTypeHistoryRepository
        {
            get
            {
                if (_policyTypeHistoryRepository == null)
                {
                    _policyTypeHistoryRepository = new PolicyTypeHistoryRepository(_context);
                    return _policyTypeHistoryRepository;
                }

                return _policyTypeHistoryRepository;
            }

        }public IAlertRepository AlertRepository
        {
            get
            {
                if (_alertRepository == null)
                {
                    _alertRepository = new AlertRepository(_context);
                    return _alertRepository;
                }

                return _alertRepository;
            }
        }

        
       
        public ICertificatePolicyRepository CertificatePolicyRepository
        {
            get
            {
                if (_certificatePolicyRepository == null)
                {
                    _certificatePolicyRepository = new CertificatePolicyRepository(_context);
                    return _certificatePolicyRepository;
                }

                return _certificatePolicyRepository;
            }
        }

        public INTPServerRepository NTPServerRepository
        {
            get
            {
                if (_ntpServerRepository == null)
                {
                    _ntpServerRepository = new NTPServerRepository(_context);
                    return _ntpServerRepository;
                }

                return _ntpServerRepository;
            }
        }

        public INTPServerHistoryRepository NTPServerHistoryRepository
        {
            get
            {
                if (_ntpServerHistoryRepository == null)
                {
                    _ntpServerHistoryRepository = new NTPServerHistoryRepository(_context);
                    return _ntpServerHistoryRepository;
                }

                return _ntpServerHistoryRepository;
            }
        }

        public ICertificateRepository CertificateRepository
        {
            get
            {
                if (_certificateRepository == null)
                {
                    _certificateRepository = new CertificateRepository(_context);
                    return _certificateRepository;
                }

                return _certificateRepository;
            }
        }

        public IDeltaRepository DeltaRepository
        {
            get
            {
                if (_deltaRepository == null)
                {
                    _deltaRepository = new DeltaRepository(_context);
                    return _deltaRepository;
                }
                return _deltaRepository;
            }
        }

        public IDeltaHistoryRepository DeltaHistoryRepository
        {
            get
            {
                if (_deltaHistoryRepository == null)
                {
                    _deltaHistoryRepository = new DeltaHistoryRepository(_context);
                    return _deltaHistoryRepository;
                }
                return _deltaHistoryRepository;
            }
        }

        public IDeltaUserRepository DeltaUserRepository
        {
            get
            {
                if (_deltaUserRepository == null)
                {
                    _deltaUserRepository = new DeltaUserRepository(_context);
                    return _deltaUserRepository;
                }
                return _deltaUserRepository;
            }
        }

        public IDeltaUserHistoryRepository DeltaUserHistoryRepository
        {
            get
            {
                if (_deltaUserHistoryRepository == null)
                {
                    _deltaUserHistoryRepository = new DeltaUserHistoryRepository(_context);
                    return _deltaUserHistoryRepository;
                }
                return _deltaUserHistoryRepository;
            }
        } 

        public IDeltaTypeRepository DeltaTypeRepository
        {
            get
            {
                if (_deltaTypeRepository == null)
                {
                    _deltaTypeRepository = new DeltaTypeRepository(_context);
                    return _deltaTypeRepository;
                }
                return _deltaTypeRepository;
            }
        }

        public IDeltaTypeHistoryRepository DeltaTypeHistoryRepository
        {
            get
            {
                if(_deltaTypeHistoryRepository == null)
                {
                    _deltaTypeHistoryRepository = new DeltaTypeHistoryRepository(_context);
                    return _deltaTypeHistoryRepository;
                }
                return _deltaTypeHistoryRepository;
            }
        }

        public ICertificatesHistoryRepository CertificatesHistoryRepository
        {
            get
            {
                if (_certificatesHistoryRepository == null)
                {
                    _certificatesHistoryRepository = new CertificatesHistoryRepository(_context);
                    return _certificatesHistoryRepository;
                }

                return _certificatesHistoryRepository;
            }
        }

        public ICertificateProfilesHistoryRepository CertificateProfilesHistoryRepository
        {
            get
            {
                if (_certificateProfilesHistoryRepository == null)
                {
                    _certificateProfilesHistoryRepository = new CertificateProfilesHistoryRepository(_context);
                    return _certificateProfilesHistoryRepository;
                }

                return _certificateProfilesHistoryRepository;
            }
        }

        public ICertificatePoliciesHistoryRepository CertificatePoliciesHistoryRepository
        {
            get
            {
                if (_certificatePoliciesHistoryRepository == null)
                {
                    _certificatePoliciesHistoryRepository = new CertificatePoliciesHistoryRepository(_context);
                    return _certificatePoliciesHistoryRepository;
                }

                return _certificatePoliciesHistoryRepository;
            }
        }

        public ICertificateProfileRepository CertificateProfileRepository
        {
            get
            {
                if (_certificateProfileRepository == null)
                {
                    _certificateProfileRepository = new CertificateProfileRepository(_context);
                    return _certificateProfileRepository;
                }

                return _certificateProfileRepository;
            }
        }

        
        public IAlertUserRepository AlertUserRepository
        {
            get
            {
                if (_alertUserRepository == null)
                {
                    _alertUserRepository = new AlertUserRepository(_context);
                    return _alertUserRepository;
                }

                return _alertUserRepository;
            }
        }
        public IAlertUserHistoryRepository AlertUserHistoryRepository
        {
            get
            {
                if (_alertUserHistoryRepository == null)
                {
                    _alertUserHistoryRepository = new AlertUserHistoryRepository(_context);
                    return _alertUserHistoryRepository;
                }

                return _alertUserHistoryRepository;
            }
        }

        public IMessageRepository MessageRepository
        {
            get
            {
                if (_messageRepository == null)
                {
                    _messageRepository = new MessageRepository(_context);
                    return _messageRepository;
                }

                return _messageRepository;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public async Task RollBackAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
