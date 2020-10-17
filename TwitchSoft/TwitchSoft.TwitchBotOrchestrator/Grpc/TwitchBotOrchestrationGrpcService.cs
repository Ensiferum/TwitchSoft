using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.SignalR;
using TwitchSoft.Shared.Services.Repository.Interfaces;
using TwitchSoft.TwitchBotOrchestrator.Hubs;
using static TwitchBotOrchestratorGrpc;

namespace TwitchSoft.TwitchBotOrcherstration.Grpc
{
    public class TwitchBotOrchestrationGrpcService : TwitchBotOrchestratorGrpcBase
    {
        private readonly IHubContext<OrchestrationHub> hub;
        private readonly IUsersRepository usersRepository;

        public TwitchBotOrchestrationGrpcService(
            IHubContext<OrchestrationHub> hub,
            IUsersRepository usersRepository)
        {
            this.hub = hub;
            this.usersRepository = usersRepository;
        }
        public override async Task<Empty> JoinChannel(JoinChannelRequest request, ServerCallContext context)
        {
            await OrchestrationHub.AddChannel(hub.Clients, request.Channelname);
            return new Empty();
        }

        public override async Task<Empty> RefreshChannels(Empty request, ServerCallContext context)
        {
            var channels = await usersRepository.GetChannelsToTrack();
            await OrchestrationHub.TriggerReconnect(hub.Clients, channels.Select(_ => _.Username));
            return new Empty();
        }
    }
}
