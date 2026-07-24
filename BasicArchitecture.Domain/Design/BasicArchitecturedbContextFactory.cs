using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BasicArchitecture.Domain.Design;

public class BasicArchitecturedbContextFactory : IDesignTimeDbContextFactory<MyDbContext.BasicArchitecturedbContext>
{
    public MyDbContext.BasicArchitecturedbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection not found (appsettings.Development.json or an environment variable is required).");

        var optionsBuilder = new DbContextOptionsBuilder<MyDbContext.BasicArchitecturedbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        return new MyDbContext.BasicArchitecturedbContext(optionsBuilder.Options);
    }
}
