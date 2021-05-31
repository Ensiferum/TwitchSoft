using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace TwitchSoft.Shared.Extensions
{
    public static class ScopeFactoryExtensions
    {
        public static async Task RunInScope(this IServiceScopeFactory scopeFactory, Func<IServiceScope, Task> action)
        {
            using(var scope = scopeFactory.CreateScope())
            {
                try
                {
                    await action(scope);
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetService<ILoggerFactory>().CreateLogger(typeof(ScopeFactoryExtensions));
                    logger.LogError(ex, "Error in RunInScope ocured");
                    throw;
                } 
            }
        }
    }
}
