using Finbuckle.MultiTenant;
using Infrastructure.Context;
using Infrastructure.Identity.Auth;
using Infrastructure.Identity.Models;
using Infrastructure.Tenancy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class Startup
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
        {
            return services.AddDbContext<TenantDbContext>(options => options
            .UseSqlServer(config.GetConnectionString("DefaultConnection")))
                .AddMultiTenant<ABSchoolTenantInfo>()
                .WithHeaderStrategy(TenancyConstans.TenantIdName)
                .WithClaimStrategy(TenancyConstans.TenantIdName)
                .WithEFCoreStore<TenantDbContext, ABSchoolTenantInfo>()
                .Services
                .AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(config.GetConnectionString("DefaultConnection")))
                .AddTransient<ITenantDbSeeder,TenantDbSeeder>()
                .AddTransient<ApplicationDbSeeder>()
                .AddIdentityService()
                .AddPermissions();
        }
        public static async Task AddDatabaseInitializer(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
        {
            using var scope = serviceProvider.CreateScope();
            await scope.ServiceProvider.GetRequiredService<ITenantDbSeeder>()
                .InitializeDatabaseAsync(cancellationToken);
        }
        internal static IServiceCollection AddIdentityService(this IServiceCollection services)
        {
            return services.AddIdentity<ApplicationUser,ApplicationRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders().Services;

        }
        internal static IServiceCollection AddPermissions(this IServiceCollection services)
        {
            return services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
                .AddScoped<IAuthorizationHandler, PermissionAuthorzationHandler>();
        }
        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
            return app
                .UseMultiTenant();
        }
    }
}
