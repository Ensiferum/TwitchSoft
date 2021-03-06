using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using TwitchSoft.Shared.Logging;
using TwitchSoft.TwitchBotOrchestrator;

CreateHostBuilder(args).Build().Run();

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureLogger()
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(5001, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http2;
                });

                options.ListenAnyIP(80, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http1;
                });
            });
            webBuilder.UseStartup<Startup>();
        });
