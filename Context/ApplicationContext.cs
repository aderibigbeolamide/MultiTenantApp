using Microsoft.EntityFrameworkCore;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using MultiTenantApp.Model; 
public class ApplicationDbContext : MultiTenantDbContext
{
    public ApplicationDbContext(TenantInfo tenantInfo, DbContextOptions<ApplicationDbContext> options)
        : base(tenantInfo, options)
    {
    }

    public DbSet<TenantInfo> Tenants { get; set; }
}
