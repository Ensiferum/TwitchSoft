using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.SignalR;
using TwitchSoft.TwitchBotOrchestrator.Hubs;
using static TwitchBotOrchestrationGrpc;

namespace TwitchSoft.TwitchBotOrcherstration.Grpc
{
    public class TwitchBotOrchestrationGrpcService : TwitchBotOrchestrationGrpcBase
    {
        private readonly IHubContext<OrchestrationHub> hub;

        public TwitchBotOrchestrationGrpcService(
            IHubContext<OrchestrationHub> hub)
        {
            this.hub = hub;
        }
        public override Task<Empty> JoinChannel(JoinChannelRequest request, ServerCallContext context)
        {
            //twitchBot.JoinChannel(request.Channelname);
            return Task.FromResult(new Empty());
        }

        public override async Task<Empty> RefreshChannels(Empty request, ServerCallContext context)
        {
            //await twitchBot.RefreshJoinedChannels();
            return new Empty();
        }
    }
}
