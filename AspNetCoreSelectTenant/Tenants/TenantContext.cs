using Microsoft.EntityFrameworkCore;

namespace AspNetCoreSelectTenant.Tenants;

public class TenantContext : DbContext
{
    public TenantContext(DbContextOptions<TenantContext> options)
        : base(options)
    {
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Tenant>().HasKey(m => m.Id);

        base.OnModelCreating(builder);
    }
}
