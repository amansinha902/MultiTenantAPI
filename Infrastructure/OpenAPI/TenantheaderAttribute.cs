using Infrastructure.Tenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.OpenAPI
{
    public class TenantHeaderAttribute() 
        : SwaggerHeaderAttribute(
            headerName: TenancyConstans.TenantIdName, 
           description: "Enter your Tenant Name",
           isRequired: true,
           defaultValue: string.Empty)
    {
    }
}
