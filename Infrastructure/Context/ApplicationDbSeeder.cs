using Finbuckle.MultiTenant.Abstractions;
using Infrastructure.Constants;
using Infrastructure.Identity.Models;
using Infrastructure.Tenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Context
{
    public class ApplicationDbSeeder(ApplicationDbContext applicationDbContext, IMultiTenantContextAccessor<ABSchoolTenantInfo> tenantInfoContextAccessor, RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;
        private readonly IMultiTenantContextAccessor<ABSchoolTenantInfo> _tenantInfoContextAccessor = tenantInfoContextAccessor;
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task InitializeDatabaseAsync(CancellationToken cancellationToken)
        {
            if (_applicationDbContext.Database.GetMigrations().Any())
            {
                if ((await _applicationDbContext.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
                {
                    await _applicationDbContext.Database.MigrateAsync(cancellationToken);
                }
                if (await _applicationDbContext.Database.CanConnectAsync(cancellationToken))
                {
                    await InitializeDefaultRoleAsync(cancellationToken);
                    await InitializeAdminUserAsync();
                }
            }
        }
        private async Task InitializeDefaultRoleAsync(CancellationToken cancellationToken)
        {
            foreach (var roleName in RoleConstants.DefaultRoles)
            {
                if (await _roleManager.Roles.SingleOrDefaultAsync(r => r.Name == roleName, cancellationToken) is not ApplicationRole incomingRole)
                {
                    incomingRole = new ApplicationRole { Name = roleName, Description = $"{roleName} Role" };
                    await _roleManager.CreateAsync(incomingRole);
                }
                if (roleName == RoleConstants.Basic)
                {
                    await AssignPermissionsToRole(SchoolPermissions.Basic, incomingRole, cancellationToken);
                }
                else if (roleName == RoleConstants.Admin)
                {
                    await AssignPermissionsToRole(SchoolPermissions.Admin, incomingRole, cancellationToken);
                    if (_tenantInfoContextAccessor.MultiTenantContext.TenantInfo.Id == TenancyConstans.Root.Id) 
                    {
                        await AssignPermissionsToRole(SchoolPermissions.Root, incomingRole, cancellationToken);
                    }
                }
            }
        }
        private async Task AssignPermissionsToRole(IReadOnlyList<SchoolPermission> rolePermission, ApplicationRole role, CancellationToken cancellationToken)
        {
            var currentClaims = await _roleManager.GetClaimsAsync(role);
            foreach (var permission in rolePermission)
            {
                if (!currentClaims.Any(c => c.Type == ClaimConstants.Permission && c.Value == permission.Name))
                {
                    await _applicationDbContext.RoleClaims.AddAsync(new ApplicationRoleClaim
                    {
                        RoleId = role.Id,
                        ClaimType = ClaimConstants.Permission,
                        ClaimValue = permission.Name,
                        Description = permission.Description,
                        Group = permission.Group
                    }, cancellationToken);

                    await _applicationDbContext.SaveChangesAsync(cancellationToken);
                }
            }
        }
        private async Task InitializeAdminUserAsync() 
        {
            if (string.IsNullOrEmpty(_tenantInfoContextAccessor.MultiTenantContext.TenantInfo.Email)) return;
            
            if(await _userManager.Users.SingleOrDefaultAsync(u =>u.Email == _tenantInfoContextAccessor.MultiTenantContext.TenantInfo.Email) is not ApplicationUser incomingUser)
            {
                incomingUser = new ApplicationUser
                {
                    UserName = _tenantInfoContextAccessor.MultiTenantContext.TenantInfo.Email,
                    Email = _tenantInfoContextAccessor.MultiTenantContext.TenantInfo.Email,
                    FisrtName = _tenantInfoContextAccessor.MultiTenantContext.TenantInfo.FirstName,
                    LastName = _tenantInfoContextAccessor.MultiTenantContext.TenantInfo.LastName,
                    EmailConfirmed = true,
                    NormalizedUserName = _tenantInfoContextAccessor.MultiTenantContext.TenantInfo.Email.ToUpper(),
                    NormalizedEmail = _tenantInfoContextAccessor.MultiTenantContext.TenantInfo.Email.ToUpper(),
                    IsActive = true,
                    PhoneNumberConfirmed = true,
                };
                var passwordHash = new PasswordHasher<ApplicationUser>();
                incomingUser.PasswordHash = passwordHash.HashPassword(incomingUser, TenancyConstans.DefaultPassword);
                await _userManager.CreateAsync(incomingUser, TenancyConstans.DefaultPassword);
            }
            if (!await _userManager.IsInRoleAsync(incomingUser, RoleConstants.Admin))
            {
                await _userManager.AddToRoleAsync(incomingUser, RoleConstants.Admin);
            }
        }
    }
}
