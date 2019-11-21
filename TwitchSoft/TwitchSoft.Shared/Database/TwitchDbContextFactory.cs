using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace TwitchSoft.Shared.Database
{
    public class TwitchDbContextFactory : IDesignTimeDbContextFactory<TwitchDbContext>
    {
        public TwitchDbContext CreateDbContext(string[] args)
        {
            // Build config
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // Get connection string
            var optionsBuilder = new DbContextOptionsBuilder<TwitchDbContext>();
            var connectionString = config.GetConnectionString(nameof(TwitchDbContext));
            optionsBuilder.UseSqlServer(connectionString);

            return new TwitchDbContext(optionsBuilder.Options);
        }
    }
}
