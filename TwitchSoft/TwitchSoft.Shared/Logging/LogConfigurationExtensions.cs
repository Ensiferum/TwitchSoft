using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Hosting;
using NLog.Extensions.Logging;

namespace TwitchSoft.Shared.Logging
{
    public static class LogConfigurationExtensions
    {
        public static IHostBuilder ConfigureLogger(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureLogging(logging =>
             {
                 logging.ClearProviders();
                 logging.SetMinimumLevel(LogLevel.Trace);
             })
            .UseNLog();
        }
    }
}
