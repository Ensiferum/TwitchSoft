using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace TwitchSoft.TelegramBot
{
    public class TelegramBotService : IHostedService
    {
        private readonly TelegramBot telegramBot;

        private ILogger<TelegramBotService> Logger { get; }
        public TelegramBotService(ILogger<TelegramBotService> logger, TelegramBot telegramBot)
        {
            Logger = logger;
            this.telegramBot = telegramBot;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("TelegramBot is starting.");
            telegramBot.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("TelegramBot is stopping.");
            telegramBot.Stop();
            return Task.CompletedTask;
        }
    }
}
