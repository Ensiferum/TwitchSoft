using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace TwitchSoft.Shared.Services.Extensions
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
                    var logger = scope.ServiceProvider.GetService<ILogger>();
                    logger.LogError(ex, "Error in RunInScope ocured");
                    throw;
                } 
            }
        }
    }
}
