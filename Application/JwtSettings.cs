using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public class JwtSettings
    {
        public string Secret { get; set; }
        public int TokenExpirationInMinute { get; set; }
        public int RefreshTokenExpirationTimeInDays { get; set; }
    }
}
