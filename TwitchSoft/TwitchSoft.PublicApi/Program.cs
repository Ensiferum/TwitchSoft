using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using TwitchSoft.PublicApi;
using TwitchSoft.Shared.Logging;

CreateHostBuilder(args).Build().Run();

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureLogger()
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
