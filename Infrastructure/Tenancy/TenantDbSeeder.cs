using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Tenancy
{
    public class TenantDbSeeder : ITenantDbSeeder
    {
        private readonly TenantDbContext _tenantDbContext;
        private readonly IServiceProvider _serviceProvider;
        public TenantDbSeeder(TenantDbContext tenantDbContext, IServiceProvider serviceProvider)
        {
            _tenantDbContext = tenantDbContext;
            _serviceProvider = serviceProvider;
        }
        public async Task InitializeDatabaseAsync(CancellationToken cancellationToken)
        {
            await InitializeDatabasWithTenantAsync(cancellationToken);
            foreach (var tenant in await _tenantDbContext.TenantInfo.ToListAsync(cancellationToken))
            {
                await InitializeAppicationDbForTenantAsync(tenant, cancellationToken);
            }
        }
        private async Task InitializeDatabasWithTenantAsync(CancellationToken ct)
        {
            if(await _tenantDbContext.TenantInfo.FindAsync([TenancyConstans.Root.Id],ct) is null)
            {
                var rootTenant = new ABSchoolTenantInfo
                {
                    Id = TenancyConstans.Root.Id,
                    Identifier = TenancyConstans.Root.Id,
                    Name = TenancyConstans.Root.Name,
                    Email = TenancyConstans.Root.Email,
                    FirstName = TenancyConstans.FirstNamr,
                    LastName = TenancyConstans.LastName,
                    IsActive = true,
                    ValidUpto = DateTime.UtcNow.AddYears(2),
                };  
                await _tenantDbContext.TenantInfo.AddAsync(rootTenant, ct);
                await _tenantDbContext.SaveChangesAsync(ct);
            }
        }
        private async Task InitializeAppicationDbForTenantAsync(ABSchoolTenantInfo currentTenant,CancellationToken ct)
        {
            using var scope = _serviceProvider.CreateScope();
            _serviceProvider.GetRequiredService<IMultiTenantContextSetter>()
                .MultiTenantContext = new MultiTenantContext<ABSchoolTenantInfo>()
                {
                    TenantInfo = currentTenant,
                };    
            await scope.ServiceProvider.GetRequiredService<ApplicationDbSeeder>().InitializeDatabaseAsync(ct);

        }
    }
}
