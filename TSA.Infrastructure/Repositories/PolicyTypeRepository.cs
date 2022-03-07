using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TSA.Infrastructure.Data;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;

namespace TSA.Infrastructure.Repositories
{
    public class PolicyTypeRepository : GenericRepository<PoliciesType>, IPolicyTypeRepository
    {
        private readonly TSADbContext _context;
        public PolicyTypeRepository(TSADbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PoliciesType> GetPolicyTypeById(int id)
        {
            var policyType = await _entities.FindAsync(id);
            return policyType;
        }

        public IEnumerable<SelectListItem> GetPoliciesTypesList()
        {
            return _context.PoliciesTypes
                .Where(x => x.IsActive == true)
                .Select(i => new SelectListItem()
                {
                    Text = i.Description,
                    Value = i.Id.ToString()
                });
        }
    }
}
