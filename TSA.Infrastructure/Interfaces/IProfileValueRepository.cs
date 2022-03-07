using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TSALibrary.Models;

namespace TSA.Infrastructure.Interfaces
{
    public interface IProfileValueRepository : IGenericRepository<ProfileValue>
    {
        public Task<ProfileValue> GetProfileValueById(int id);
        public IEnumerable<SelectListItem> GetProfilesValuesList();
        public IList<ProfileValue> GetProfilesValuesByTypeList(int profileTypeId);
    }
}
