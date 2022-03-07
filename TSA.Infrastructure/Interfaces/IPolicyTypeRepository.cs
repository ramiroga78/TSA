using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using TSA.Infrastructure.Entities;
using TSALibrary.Models;

namespace TSA.Infrastructure.Interfaces
{
    public interface IPolicyTypeRepository : IGenericRepository<PoliciesType>
    {
        public Task<PoliciesType> GetPolicyTypeById(int id);

        public IEnumerable<SelectListItem> GetPoliciesTypesList();

    }
}
