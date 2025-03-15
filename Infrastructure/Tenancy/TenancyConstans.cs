using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Tenancy
{
    public class TenancyConstans
    {
        public const string TenantIdName = "tenant";

        public const string DefaultPassword = "Aman@ss123";
        public const string FirstNamr = "Aman";
        public const string LastName = "Sinha";

        public static class Root
        {
            public const string Id = "root";
            public const string Name = "Root";
            public const string Email = "admin.root@abschool.com";
        }
    }
}
