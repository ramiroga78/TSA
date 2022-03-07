using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using TSA.Infrastructure.Data;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;

namespace TSA.Infrastructure.Repositories
{
    public class DeltaUserHistoryRepository : GenericRepository<DeltaUserHistory>, IDeltaUserHistoryRepository
    {
        public DeltaUserHistoryRepository(TSADbContext context) : base(context)
        {
        }
    }
}
