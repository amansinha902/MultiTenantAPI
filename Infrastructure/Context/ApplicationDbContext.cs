using Domain.Entities;
using Finbuckle.MultiTenant.Abstractions;
using Infrastructure.Tenancy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Context
{
    public class ApplicationDbContext : BaseDbContext
    {
        public ApplicationDbContext(IMultiTenantContextAccessor<ABSchoolTenantInfo> tenantInfoContextAccessor, 
            DbContextOptions options) : base(tenantInfoContextAccessor, options)
        {}
        public DbSet<School> Schools => Set<School>();
    }
}
