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
        private Timer _channelsJoinedCheckerTimer;
        private Timer _connectionCheckerTimer;

        private ILogger<TwitchBotService> Logger { get; }
        public TwitchBotService(
            ILogger<TwitchBotService> logger,
            TwitchBot twitchBot)
        {
            this.Logger = logger;
            this.twitchBot = twitchBot;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("TwitchBotService is starting.");
            twitchBot.Start();

            _channelsJoinedCheckerTimer = new Timer(CheckJoinedChannels, null, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(60));
            _connectionCheckerTimer = new Timer(CheckConnection, null, TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(10));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _channelsJoinedCheckerTimer?.Change(Timeout.Infinite, 0);
            _connectionCheckerTimer?.Change(Timeout.Infinite, 0);

            Logger.LogInformation("TwitchBotService is stopping.");
            twitchBot.Stop();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _channelsJoinedCheckerTimer?.Dispose();
            _connectionCheckerTimer?.Dispose();
        }

        private async void CheckJoinedChannels(object state)
        {
            Logger.LogTrace("CheckJoinedChannels");
            await twitchBot.TriggerChannelsJoin();
        }

        private async void CheckConnection(object state)
        {
            Logger.LogTrace("CheckConnection");
            await twitchBot.CheckIfStillConnected();
        }
    }
}
