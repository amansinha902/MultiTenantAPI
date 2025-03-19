using Application.Features.Tenancy;
using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant;
using Infrastructure.Context;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapster;

namespace Infrastructure.Tenancy
{
    public class TenantService : ITenantService
    {
        private readonly IMultiTenantStore<ABSchoolTenantInfo> _tenantStore;
        private readonly ApplicationDbSeeder _dbSeeder;
        private readonly IServiceProvider _serviceProvider;

        public TenantService(IMultiTenantStore<ABSchoolTenantInfo> tenantStore, ApplicationDbSeeder dbSeeder, IServiceProvider serviceProvider)
        {
            _tenantStore = tenantStore;
            _dbSeeder = dbSeeder;
            _serviceProvider = serviceProvider;
        }

        public async Task<string> ActivateAsync(string id)
        {
            var tenantInDb = await _tenantStore.TryGetAsync(id);
            tenantInDb.IsActive = true;

            await _tenantStore.TryUpdateAsync(tenantInDb);
            return tenantInDb.Identifier;
        }

        public async Task<string> CreateTenantAsync(CreateTenantRequest createTenant, CancellationToken ct)
        {
            var newTenant = new ABSchoolTenantInfo
            {
                Id = createTenant.Identifier,
                Identifier = createTenant.Identifier,
                Name = createTenant.Name,
                IsActive = createTenant.IsActive,
                ConnectionString = createTenant.ConnectionString,
                Email = createTenant.Email,
                FirstName = createTenant.FirstName,
                LastName = createTenant.LastName,
                ValidUpto = createTenant.ValidUpTo
            };

            await _tenantStore.TryAddAsync(newTenant);

            using var scope = _serviceProvider.CreateScope();

            _serviceProvider.GetRequiredService<IMultiTenantContextSetter>()
                .MultiTenantContext = new MultiTenantContext<ABSchoolTenantInfo>()
                {
                    TenantInfo = newTenant
                };
            await scope.ServiceProvider.GetRequiredService<ApplicationDbSeeder>()
                .InitializeDatabaseAsync(ct);

            return newTenant.Identifier;
        }

        public async Task<string> DeactivateAsync(string id)
        {
            var tenantInDb = await _tenantStore.TryGetAsync(id);
            tenantInDb.IsActive = false;

            await _tenantStore.TryUpdateAsync(tenantInDb);
            return tenantInDb.Identifier;
        }
        public async Task<TenantResponse> GetTenantByIdAsync(string id)
        {
            var tenantInDb = await _tenantStore.TryGetAsync(id);

            #region Manual Mapping
            //var tenantResponse = new TenantResponse
            //{
            //    Identifier = tenantInDb.Identifier,
            //    Name = tenantInDb.Name,
            //    ConnectionString = tenantInDb.ConnectionString,
            //    Email = tenantInDb.Email,
            //    FirstName = tenantInDb.FirstName,
            //    LastName = tenantInDb.LastName,
            //    IsActive = tenantInDb.IsActive,
            //    ValidUpTo = tenantInDb.ValidUpTo
            //};
            //return tenantResponse;
            #endregion
            // Mapster
            return tenantInDb.Adapt<TenantResponse>();

        }

        public async Task<List<TenantResponse>> GetTenantAsync()
        {
            var tenantsInDb = await _tenantStore.GetAllAsync();
            return tenantsInDb.Adapt<List<TenantResponse>>();
        }

        public async Task<string> UpdateSubscrptionAsync(UpdateTenantSubscriptionRequest updateTenantSubscription)
        {
            var tenantInDb = await _tenantStore.TryGetAsync(updateTenantSubscription.TenantId);

            tenantInDb.ValidUpto = updateTenantSubscription.NewExpiryDate;

            await _tenantStore.TryUpdateAsync(tenantInDb);

            return tenantInDb.Identifier;
        }
    }
}
