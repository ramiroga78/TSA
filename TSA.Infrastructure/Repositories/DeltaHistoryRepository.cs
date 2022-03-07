using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSA.Infrastructure.Data;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;

namespace TSA.Infrastructure.Repositories
{
   public class DeltaHistoryRepository : GenericRepository<DeltaHistory>, IDeltaHistoryRepository
    {
        public DeltaHistoryRepository(TSADbContext context) : base(context)
        {
        }

        public void SaveChangesDeltaHistory(DeltaHistory deltaHistory)
        {
            //save history
        }
    }
}
