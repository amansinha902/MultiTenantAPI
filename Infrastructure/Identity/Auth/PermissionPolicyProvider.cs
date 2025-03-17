using Infrastructure.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Infrastructure.Identity.Auth
{
    public class PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : IAuthorizationPolicyProvider
    {
        public DefaultAuthorizationPolicyProvider FallBackPolicyProvider { get; } = new DefaultAuthorizationPolicyProvider(options);
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
          return FallBackPolicyProvider.GetDefaultPolicyAsync();
        }

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
        {
            return Task.FromResult<AuthorizationPolicy>(null);
        }

        public Task<AuthorizationPolicy> GetPolicyAsync(string permission)
        {
            if(permission.StartsWith(ClaimConstants.Permission,StringComparison.OrdinalIgnoreCase))
            {
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new PermissionRequirement(permission));
                return Task.FromResult(policy.Build());
            }
            return FallBackPolicyProvider.GetPolicyAsync(permission);
        }
    }
}
