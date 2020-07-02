using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TwitchSoft.TwitchBot
{
    public class TwitchBotService : IHostedService
    {
        private readonly TwitchBot twitchBot;

        private ILogger<TwitchBotService> Logger { get; }
        public TwitchBotService(
            ILogger<TwitchBotService> logger,
            TwitchBot twitchBot)
        {
            this.Logger = logger;
            this.twitchBot = twitchBot;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("TwitchBotService is starting.");
            await twitchBot.Start();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("TwitchBotService is stopping.");
            await twitchBot.Stop();
        }
    }
}
