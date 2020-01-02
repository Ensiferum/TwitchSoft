using Coravel.Invocable;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using static TelegramBotGrpc;

namespace TwitchSoft.Maintenance.Jobs
{
    public class SentDailyMessageDigest: IInvocable
    {
        private readonly ILogger<SentDailyMessageDigest> logger;
        private readonly string rootUserChatId;

        public SentDailyMessageDigest(
            ILogger<SentDailyMessageDigest> logger,
            IConfiguration config)
        {
            rootUserChatId = config.GetValue<string>("JobConfigs:RootUserChatId");
            this.logger = logger;
        }

        public async Task Invoke()
        {
            logger.LogInformation($"Start executing job: {nameof(SentDailyMessageDigest)}");

            Channel grpcChannel = new Channel("telegrambot", 80, ChannelCredentials.Insecure);
            var client = new TelegramBotGrpcClient(grpcChannel);
            await client.SentDayDigestAsync(new DigestInfoRequest
            {
                ChatId = rootUserChatId,
            });
            logger.LogInformation($"End executing job: {nameof(SentDailyMessageDigest)}");
        }
    }
}
