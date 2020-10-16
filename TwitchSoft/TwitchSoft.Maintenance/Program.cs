using Microsoft.Extensions.Hosting;
using TwitchSoft.Shared.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using System;
using TwitchSoft.Maintenance;

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
CreateHostBuilder(args).Build().Run();

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureLogger()
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
