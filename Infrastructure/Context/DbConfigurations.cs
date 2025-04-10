﻿using Domain.Entities;
using Finbuckle.MultiTenant;
using Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Context
{
    internal class DbConfigurations
    {
        internal class ApplicationUserConfig : IEntityTypeConfiguration<ApplicationUser>
        {
            public void Configure(EntityTypeBuilder<ApplicationUser> builder)
            {
                builder.
                    ToTable("Users", "Identity")
                    .IsMultiTenant();
            }
        }
        internal class ApplicationRoleConfig : IEntityTypeConfiguration<ApplicationRole>
        {
            public void Configure(EntityTypeBuilder<ApplicationRole> builder)
            {
                builder.
                    ToTable("Roles", "Identity")
                    .IsMultiTenant();
            }
        }
        internal class ApplicationRoleClaimsConfig : IEntityTypeConfiguration<ApplicationRoleClaim>
        {
            public void Configure(EntityTypeBuilder<ApplicationRoleClaim> builder)
            {
                builder.
                    ToTable("RolesClaims", "Identity")
                    .IsMultiTenant();
            }
        }
        internal class IdentityUserRoleConfig : IEntityTypeConfiguration<IdentityUserRole<string>>
        {
            public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
            {
                builder.
                    ToTable("UserRoles", "Identity")
                    .IsMultiTenant();
            }
        }
        internal class IdentityUserClaimConfig : IEntityTypeConfiguration<IdentityUserClaim<string>>
        {
            public void Configure(EntityTypeBuilder<IdentityUserClaim<string>> builder)
            {
                builder.
                    ToTable("UserClaims", "Identity")
                    .IsMultiTenant();
            }
        }
        internal class IdentityUserLoginConfig : IEntityTypeConfiguration<IdentityUserLogin<string>>
        {
            public void Configure(EntityTypeBuilder<IdentityUserLogin<string>> builder)
            {
                builder.
                    ToTable("UserLogin", "Identity")
                    .IsMultiTenant();
            }
        }
        internal class IdentityUserTokenConfig : IEntityTypeConfiguration<IdentityUserToken<string>>
        {
            public void Configure(EntityTypeBuilder<IdentityUserToken<string>> builder)
            {
                builder.
                    ToTable("UserToken", "Identity")
                    .IsMultiTenant();
            }
        }
        internal class SchoolConfig : IEntityTypeConfiguration<School>
        {
            public void Configure(EntityTypeBuilder<School> builder)
            {
                builder.
                    ToTable("Schools", "Academics")
                    .IsMultiTenant();
                builder.
                    Property(sc => sc.Name).IsRequired().HasMaxLength(60);
            }
        }
    }
}
