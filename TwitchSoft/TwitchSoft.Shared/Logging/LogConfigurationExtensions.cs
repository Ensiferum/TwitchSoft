using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Hosting;

namespace TwitchSoft.Shared.Logging
{
    public static class LogConfigurationExtensions
    {
        public static IHostBuilder ConfigureLogger(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureLogging(loggerFactory =>
             {
                 loggerFactory.ClearProviders();
                 loggerFactory.SetMinimumLevel(LogLevel.Trace);
             })
            .UseNLog();
        }
    }
}
