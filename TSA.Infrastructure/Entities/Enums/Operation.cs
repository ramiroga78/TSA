using System.Collections.Generic;
using TSA.Infrastructure.Helpers;
using TSALibrary.Models;

namespace TSA.Infrastructure.Entities.Enums
{
    public class Operation : Enumeration
    {
        public static readonly Operation Read = new(1, nameof(Read));
        public static readonly Operation Write = new(2, nameof(Write));
        public static readonly Operation Update = new(3, nameof(Update));
        public static readonly Operation Delete = new(4, nameof(Delete));

        public Operation(int id, string name)
            : base(id, name)
        {
        }

        public IEnumerable<RoleFunction> RoleFunction { get; set; }
    }
}
