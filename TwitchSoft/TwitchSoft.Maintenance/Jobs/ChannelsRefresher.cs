using Coravel.Invocable;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using static TwitchBotOrchestrationGrpc;

namespace TwitchSoft.Maintenance.Jobs
{
    public class ChannelsRefresher : IInvocable
    {
        private readonly ILogger<ChannelsRefresher> logger;
        private readonly TwitchBotOrchestrationGrpcClient twitchBotClient;

        public ChannelsRefresher(
            ILogger<ChannelsRefresher> logger,
            TwitchBotOrchestrationGrpcClient twitchBotClient)
        {
            this.logger = logger;
            this.twitchBotClient = twitchBotClient;
        }
        public async Task Invoke()
        {
            logger.LogInformation($"Start executing job: {nameof(ChannelsRefresher)}");

            await twitchBotClient.RefreshChannelsAsync(new Empty());

            logger.LogInformation($"End executing job: {nameof(ChannelsRefresher)}");
        }
    }
}
