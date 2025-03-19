using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Tenancy
{
    public class UpdateTenantSubscriptionRequest
    {
        public string TenantId { get; set; }
        public DateTime NewExpiryDate { get; set; }
    }
}
