using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Thomas.Api.Infrastructure;

namespace Thomas.Api.Infrastructure;

public class ThomasDbContextFactory : IDesignTimeDbContextFactory<ThomasDbContext>
{
    public ThomasDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddUserSecrets<Program>(optional: true)   // <- lets EF design-time use User Secrets
            .AddEnvironmentVariables()
            .Build();

        var cs = config.GetConnectionString("ThomasDb")
                 ?? throw new InvalidOperationException("Missing ConnectionStrings:ThomasDb for design time.");
        var opts = new DbContextOptionsBuilder<ThomasDbContext>()
            .UseSqlServer(cs)
            .Options;

        return new ThomasDbContext(opts);
    }
}
