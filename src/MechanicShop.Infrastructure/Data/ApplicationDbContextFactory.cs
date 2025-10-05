using MechanicShop.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BeDriver.DataAccess.Data;

public sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer("Server = .; Database = MechanicShop_DB; Trusted_Connection=True; MultipleActiveResultSets = true; TrustServerCertificate = True;");
        
         return new AppDbContext(optionsBuilder.Options, null);
     }
}