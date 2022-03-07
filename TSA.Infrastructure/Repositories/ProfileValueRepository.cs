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
    public class ProfileValueRepository : GenericRepository<ProfileValue>, IProfileValueRepository
    {
        private readonly TSADbContext _context;
        public ProfileValueRepository(TSADbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<ProfileValue> GetProfileValueById(int id)
        {
            var profileValue = await _entities.FindAsync(id);
            return profileValue;
        }

        public IEnumerable<SelectListItem> GetProfilesValuesList()
        {
            return _context.ProfileValues.Select(i => new SelectListItem()
            {
                Text = i.Value,
                Value = i.Id.ToString()
            });
        }

        public IList<ProfileValue> GetProfilesValuesByTypeList(int profileTypeId)
        {
            return _context.ProfileValues
                .Where(x => x.ProfileTypeId == profileTypeId && x.IsActive == true)
                .ToList();
        }


    }
}
