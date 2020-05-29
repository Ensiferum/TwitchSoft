using Coravel.Invocable;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TwitchSoft.Shared.Services.TwitchApi;
using static TwitchBotGrpc;

namespace TwitchSoft.Maintenance.Jobs
{
    public class ChannelsRefresher : IInvocable
    {
        private readonly ILogger<ChannelsRefresher> logger;
        private readonly string twitchBotHost;

        public ChannelsRefresher(
            ILogger<ChannelsRefresher> logger,
            IConfiguration config)
        {
            this.logger = logger;
            twitchBotHost = config.GetValue<string>("Services:TwitchBot");
        }
        public async Task Invoke()
        {
            logger.LogInformation($"Start executing job: {nameof(ChannelsRefresher)}");

            Channel grpcChannel = new Channel(twitchBotHost, 80, ChannelCredentials.Insecure);
            var client = new TwitchBotGrpcClient(grpcChannel);
            await client.RefreshChannelsAsync(new Empty());

            logger.LogInformation($"End executing job: {nameof(ChannelsRefresher)}");
        }
    }
}
