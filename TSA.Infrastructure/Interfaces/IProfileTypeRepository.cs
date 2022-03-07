using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TSALibrary.Models;

namespace TSA.Infrastructure.Interfaces
{
    public interface IProfileTypeRepository : IGenericRepository<ProfileType>
    {
        public Task<ProfileType> GetProfileTypeById(int Id);
        public IEnumerable<SelectListItem> GetProfilesTypesList();
    }
}
