using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Core.Logging;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using TwitchSoft.Shared.Services.Repository.Interfaces;
using TwitchSoft.TwitchBotOrchestrator.Hubs;
using static TwitchBotOrchestratorGrpc;

namespace TwitchSoft.TwitchBotOrcherstration.Grpc
{
    public class TwitchBotOrchestrationGrpcService : TwitchBotOrchestratorGrpcBase
    {
        private readonly IHubContext<OrchestrationHub, IOrchestrationClient> hub;
        private readonly IUsersRepository usersRepository;
        private readonly ILogger<TwitchBotOrchestrationGrpcService> logger;

        public TwitchBotOrchestrationGrpcService(
            IHubContext<OrchestrationHub, IOrchestrationClient> hub,
            IUsersRepository usersRepository,
            ILogger<TwitchBotOrchestrationGrpcService> logger)
        {
            this.hub = hub;
            this.usersRepository = usersRepository;
            this.logger = logger;
        }
        public override async Task<Empty> JoinChannel(JoinChannelRequest request, ServerCallContext context)
        {
            await OrchestrationHub.AddChannel(hub.Clients, request.Channelname, logger);
            return new Empty();
        }

        public override async Task<Empty> RefreshChannels(Empty request, ServerCallContext context)
        {
            var channels = await usersRepository.GetChannelsToTrack();
            await OrchestrationHub.RefreshChannels(hub.Clients, channels.Select(_ => _.Username), logger);
            return new Empty();
        }
    }
}
