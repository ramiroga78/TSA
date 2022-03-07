using System.Collections.Generic;
using TSA.Infrastructure.Helpers;
using TSALibrary.Models;

namespace TSA.Infrastructure.Entities.Enums
{
    public class Function : Enumeration
    {
        public static readonly Function UsersCrud = new(1, nameof(UsersCrud));
        public static readonly Function RolesCrud = new(2, nameof(RolesCrud));
        public static readonly Function CertificatesCrud = new(3, nameof(CertificatesCrud));
        public static readonly Function PoliciesCrud = new(4, nameof(PoliciesCrud));

        public Function(int id, string name)
            : base(id, name)
        {
        }

        public IEnumerable<RoleFunction> RoleFunction { get; set; }
    }
}
