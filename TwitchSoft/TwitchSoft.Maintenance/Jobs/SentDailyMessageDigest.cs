using Coravel.Invocable;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TwitchSoft.Shared;
using static TelegramBotGrpc;

namespace TwitchSoft.Maintenance.Jobs
{
    public class SentDailyMessageDigest: IInvocable
    {
        private readonly ILogger<SentDailyMessageDigest> logger;
        private readonly string rootUserChatId;
        private readonly TelegramBotGrpcClient telegramBotClient;

        public SentDailyMessageDigest(
            ILogger<SentDailyMessageDigest> logger,
            IConfiguration config, 
            TelegramBotGrpcClient telegramBotClient)
        {
            rootUserChatId = config.GetValue<string>("JobConfigs:RootUserChatId");
            this.logger = logger;
            this.telegramBotClient = telegramBotClient;
        }

        public async Task Invoke()
        {
            logger.LogInformation($"Start executing job: {nameof(SentDailyMessageDigest)}");

            await telegramBotClient.SendDayDigestAsync(new DigestInfoRequest
            {
                ChatId = rootUserChatId,
                Username = Constants.MadTwitchName,
                TwitchUserId = Constants.MadTwitchId
            });
            logger.LogInformation($"End executing job: {nameof(SentDailyMessageDigest)}");
        }
    }
}
