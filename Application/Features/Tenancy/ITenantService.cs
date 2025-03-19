using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Tenancy
{
    public interface ITenantService
    {
        Task<string> CreateTenantAsync(CreateTenantRequest createTenant, CancellationToken ct);
        Task<string> ActivateAsync(string Id);
        Task<string> DeactivateAsync(string Id);
        Task<string> UpdateSubscrptionAsync(UpdateTenantSubscriptionRequest updateTenantSubscription);
        Task<List<TenantResponse>> GetTenantAsync();
        Task<TenantResponse> GetTenantByIdAsync(string Id);
    }
}
