using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using TSA.Infrastructure.Data;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;

namespace TSA.Infrastructure.Repositories
{
    public class CertificatesHistoryRepository : GenericRepository<CertificatesHistory>, ICertificatesHistoryRepository
    {
        public CertificatesHistoryRepository(TSADbContext context) : base(context)
        {
        }
    }
}
