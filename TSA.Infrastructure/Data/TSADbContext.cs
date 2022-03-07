using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using TSA.Infrastructure.Entities;
using TSALibrary.Models;

#nullable disable

namespace TSA.Infrastructure.Data
{
    public partial class TSADbContext : DbContext
    {
        public TSADbContext()
        {
        }

        public TSADbContext(DbContextOptions<TSADbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Alert> Alerts { get; set; }
        public virtual DbSet<AlertUser> AlertUsers { get; set; }
        public virtual DbSet<AlertUserHistory> AlertUserHistories { get; set; }
        public virtual DbSet<AlertsHistory> AlertsHistories { get; set; }
        public virtual DbSet<AlertsLog> AlertsLogs { get; set; }
        public virtual DbSet<Certificate> Certificates { get; set; }
        public virtual DbSet<CertificateOrganization> CertificateOrganizations { get; set; }
        public virtual DbSet<CertificateOrganizationHistory> CertificateOrganizationHistories { get; set; }
        public virtual DbSet<CertificateOrganizationType> CertificateOrganizationTypes { get; set; }
        public virtual DbSet<CertificatePoliciesHistory> CertificatePoliciesHistories { get; set; }
        public virtual DbSet<CertificatePolicy> CertificatePolicies { get; set; }
        public virtual DbSet<CertificateProfile> CertificateProfiles { get; set; }
        public virtual DbSet<CertificateProfilesHistory> CertificateProfilesHistories { get; set; }
        public virtual DbSet<CertificatesHistory> CertificatesHistories { get; set; }
        public virtual DbSet<Delta> Deltas { get; set; }
        public virtual DbSet<DeltaHistory> DeltaHistories { get; set; }
        public virtual DbSet<DeltaUser> DeltaUsers { get; set; }
        public virtual DbSet<DeltaUserHistory> DeltaUserHistories { get; set; }
        public virtual DbSet<DeltaType> DeltaTypes { get; set; }
        public virtual DbSet<DeltaTypeHistory> DeltaTypeHistories { get; set; }
        public virtual DbSet<Function> Functions { get; set; }
        public virtual DbSet<FunctionsHistory> FunctionsHistories { get; set; }
        public virtual DbSet<IpAddress> IpAddresses { get; set; }
        public virtual DbSet<IpAddressesHistory> IpAddressesHistories { get; set; }
        public virtual DbSet<MyTemp> MyTemps { get; set; }
        public virtual DbSet<NTPServer> Ntpservers { get; set; }
        public virtual DbSet<NTPServerHistory> NtpserverHistories { get; set; }
        public virtual DbSet<PoliciesType> PoliciesTypes { get; set; }
        public virtual DbSet<PoliciesTypesHistory> PoliciesTypesHistories { get; set; }
        public virtual DbSet<ProfileType> ProfileTypes { get; set; }
        public virtual DbSet<ProfileTypesHistory> ProfileTypesHistories { get; set; }
        public virtual DbSet<ProfileValue> ProfileValues { get; set; }
        public virtual DbSet<ProfileValueHistory> ProfileValueHistories { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<RoleUser> RoleUsers { get; set; }
        public virtual DbSet<RoleUserHistory> RoleUserHistories { get; set; }
        public virtual DbSet<RoleFunction> RolesFunctions { get; set; }
        public virtual DbSet<RoleFunctionHistory> RolesFunctionsHistories { get; set; }
        public virtual DbSet<RolesHistory> RolesHistories { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserHistory> UsersHistories { get; set; }
        public virtual DbSet<RequestLog> RequestLogs { get; set; }
        public virtual DbSet<Message> Messages { get; set; }

        public virtual DbSet<ExternalUser> ExternalUsers { get; set; }
        public virtual DbSet<ExternalUserHistory> ExternalUsersHistories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Certificate>(entity =>
            {
                entity.HasOne(d => d.Issuer)
               .WithMany(p => p.CertificateIssuers)
               .HasForeignKey(d => d.IssuerId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_Certificates_CertificateOrganization");

                entity.HasOne(d => d.Subject)
               .WithMany(p => p.CertificateSubjects)
               .HasForeignKey(d => d.SubjectId)
               .HasConstraintName("FK_Certificates_CertificateOrganization1");
            });

            modelBuilder.Entity<CertificateProfile>(entity =>
            {
                entity.HasOne(d => d.ProfileValue)
               .WithMany(p => p.CertificateProfiles)
               .HasForeignKey(d => d.IdProfileValue)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_CertificateProfiles_ProfileValues");
            });

            modelBuilder.Entity<CertificatePolicy>(entity =>
            {
                entity.HasOne(d => d.PolicyType)
               .WithMany(p => p.CertificatePolicies)
               .HasForeignKey(d => d.IdPolicyType)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_CertificatePolicies_ProfileType");
            });
        }
               
    }
}