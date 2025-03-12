using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Infrastructure.Identity.Models;
using Infrastructure.Tenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design.Internal;
using System.Reflection;


namespace Infrastructure.Context
{
    public abstract class BaseDbContext :
        MultiTenantIdentityDbContext<
            ApplicationUser,
            ApplicationRole,
            string,
            IdentityUserClaim<string>,
            IdentityUserRole<string>,
            IdentityUserLogin<string>,
            ApplicationRoleClaim,
            IdentityUserToken<string>
            >
    {
        private new ABSchoolTenantInfo TenantInfo { get; set; }
        protected BaseDbContext(IMultiTenantContextAccessor<ABSchoolTenantInfo> tenantInfoContextAccessor,DbContextOptions options) : base(tenantInfoContextAccessor, options)
        {
            TenantInfo = tenantInfoContextAccessor.MultiTenantContext.TenantInfo;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if (!string.IsNullOrEmpty(TenantInfo?.ConnectionString)) 
            { 
                optionsBuilder.UseSqlServer(TenantInfo?.ConnectionString, 
                    options => options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));
            }
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);    
        }
    }
}
