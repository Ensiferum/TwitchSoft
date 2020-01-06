using Microsoft.Extensions.DependencyInjection;
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
                catch
                {
                    throw;
                } 
            }
        }
    }
}
