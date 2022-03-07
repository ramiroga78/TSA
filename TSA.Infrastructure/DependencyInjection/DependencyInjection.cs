using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TSA.Infrastructure.Interfaces;
using TSA.Infrastructure.Repositories;
using TSA.Infrastructure.Services;

namespace TSA.Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRepository(this IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IExternalUserRepository, ExternalUserRepository>();
            services.AddTransient<IProfileTypeRepository, ProfileTypeRepository>();
            services.AddTransient<IProfileTypeHistoryRepository, ProfileTypeHistoryRepository>();
            services.AddTransient<IProfileValueRepository, ProfileValueRepository>();
            services.AddTransient<IProfileValueHistoryRepository, ProfileValueHistoryRepository>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<IRoleHistoryRepository, RoleHistoryRepository>();
            services.AddTransient<IRoleUserRepository, RoleUserRepository>();
            services.AddTransient<IRoleUserHistoryRepository, RoleUserHistoryRepository>();
            services.AddTransient<ICertificateOrganizationRepository, CertificateOrganizationRepository>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<INTPServerRepository, NTPServerRepository>();
            services.AddTransient<INTPServerHistoryRepository, NTPServerHistoryRepository>();
            services.AddTransient<IPolicyTypeRepository, PolicyTypeRepository>();
            services.AddTransient<IAlertRepository, AlertRepository>();

            

            services.AddTransient<IIpAddressRepository, IpAddressRepository>();
            services.AddTransient<IIpAddressHistoryRepository, IpAddressHistoryRepository>();
           
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IRequestLogRepository, RequestLogRepository>();
            //services.AddTransient<IProfileHashAlgorithmItemRepository, ProfileHashAlgorithmItemRepository>();
            services.AddTransient<ICertificateRepository, CertificateRepository>();

            services.AddTransient<IDeltaRepository, DeltaRepository>();
            services.AddTransient<IDeltaHistoryRepository, DeltaHistoryRepository>();
            services.AddTransient<IDeltaTypeRepository, DeltaTypeRepository>();
            services.AddTransient<IDeltaTypeHistoryRepository, DeltaTypeHistoryRepository>();
            services.AddTransient<IDeltaUserRepository, DeltaUserRepository>();
            services.AddTransient<IDeltaUserHistoryRepository, DeltaUserHistoryRepository>();

            services.AddTransient<ICertificatePolicyRepository, CertificatePolicyRepository>();
            services.AddTransient<IMessageRepository, MessageRepository>();
            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IExternalUserService, ExternalUserService>();
            services.AddScoped<IProfileService, ProfileService>();

            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ICertificateOrganizationService, CertificateOrganizationService>();            
            services.AddScoped<IRequestLogService, RequestLogService>();           
            services.AddScoped<IIpAddresService, IpAddressService>();
            //services.AddScoped<IIpAddressHistoryService, IpAddressHistoryService>();
            services.AddScoped<INTPServerService, NTPServerService>();
           

            services.AddScoped<ICertificateService, CertificateService>();

            services.AddScoped<IPolicyTypeService, PolicyTypeService>();

            services.AddScoped<IDeltaService, DeltaService>();
            services.AddScoped<IAlertService, AlertService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<ICertificatePolicyService, CertificatePolicyService>();
            services.AddScoped<IMessageService, MessageService>();
            return services;
        }
    }
          

    public static class RequestResponseLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
    }
}