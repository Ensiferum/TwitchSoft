using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TwitchSoft.TwitchBot
{
    public class TwitchBotService : IHostedService, IDisposable
    {
        private readonly TwitchBot twitchBot;
        private Timer _timer;

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
            var rand = new Random();
            var secondsToWarmUp = rand.Next(0, 60);
            await Task.Delay(TimeSpan.FromSeconds(secondsToWarmUp), cancellationToken);
            twitchBot.Start();

            _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(60));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            Logger.LogInformation("TwitchBotService is stopping.");
            twitchBot.Stop();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private void DoWork(object state)
        {
            Logger.LogInformation("TriggerRefreshJoinedChannels");
            twitchBot.TriggerRefreshJoinedChannels();
        }
    }
}
