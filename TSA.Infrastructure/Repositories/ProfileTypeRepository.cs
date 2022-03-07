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
    public class ProfileTypeRepository : GenericRepository<ProfileType>, IProfileTypeRepository
    {
        private readonly TSADbContext _context;
        public ProfileTypeRepository(TSADbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<ProfileType> GetProfileTypeById(int id)
        {
            var profileType = await _entities.FindAsync(id);
            return profileType;
        }

        public IEnumerable<SelectListItem> GetProfilesTypesList()
        {
            return _context.ProfileTypes
                .Where(x => x.IsActive == true)
                .Select(i => new SelectListItem()
                {
                    Text = i.Description,
                    Value = i.Id.ToString()
                });
        }

    }
}
