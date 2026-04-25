using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Crm.Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<CrmDbContext>
{
    public CrmDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CrmDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=crm;Username=postgres;Password=secret");

        return new CrmDbContext(optionsBuilder.Options);
    }
}