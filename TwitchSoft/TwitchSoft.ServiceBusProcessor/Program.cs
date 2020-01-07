using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TwitchSoft.Shared.Database;
using TwitchSoft.Shared.Redis;
using TwitchSoft.Shared.Services.Repository;
using TwitchSoft.Shared.Services.Repository.Interfaces;
using TwitchSoft.Shared.Services.TwitchApi;

namespace TwitchSoft.ServiceBusProcessor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(loggerFactory =>
                {
                    //loggerFactory.ClearProviders();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    // Set up the objects we need to get to configuration settings
                    var Configuration = hostContext.Configuration;

                    services.AddScoped<IRepository, Repository>();
                    services.AddScoped<ITwitchApiService, TwitchApiService>();

                    services.AddDbContext<TwitchDbContext>(
                        options => options.UseSqlServer(Configuration.GetConnectionString(nameof(TwitchDbContext))));

                    services.AddServiceBusProcessors(Configuration);

                    services.AddLocalRedisCache(Configuration);
                });
    }
}
