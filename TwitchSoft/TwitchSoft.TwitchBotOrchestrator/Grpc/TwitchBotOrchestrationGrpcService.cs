using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.SignalR;
using TwitchSoft.Shared.Services.Repository.Interfaces;
using TwitchSoft.TwitchBotOrchestrator.Hubs;
using static TwitchBotOrchestrationGrpc;

namespace TwitchSoft.TwitchBotOrcherstration.Grpc
{
    public class TwitchBotOrchestrationGrpcService : TwitchBotOrchestrationGrpcBase
    {
        private readonly IHubContext<OrchestrationHub> hub;
        private readonly IRepository repository;

        public TwitchBotOrchestrationGrpcService(
            IHubContext<OrchestrationHub> hub,
            IRepository repository)
        {
            this.hub = hub;
            this.repository = repository;
        }
        public override async Task<Empty> JoinChannel(JoinChannelRequest request, ServerCallContext context)
        {
            await OrchestrationHub.AddChannel(hub.Clients, request.Channelname);
            return new Empty();
        }

        public override async Task<Empty> RefreshChannels(Empty request, ServerCallContext context)
        {
            var channels = await repository.GetChannelsToTrack();
            await OrchestrationHub.TriggerReconnect(hub.Clients, channels.Select(_ => _.Username));
            return new Empty();
        }
    }
}
