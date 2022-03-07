using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TSALibrary.Models;

namespace TSA.Infrastructure.ViewModels
{
    public class UsersAndRolesVM
    {
        public User User { get; set; }
        public int RoleId { get; set; }
        public IEnumerable<SelectListItem> RolesList { get; set; }
    }
}
