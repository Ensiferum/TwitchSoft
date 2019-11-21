using System.Threading.Tasks;
using Grpc.Core;
using static TwitchBotGrpc;

namespace TwitchSoft.TwitchBot.Grpc
{
    public class TwitchBotGrpcService : TwitchBotGrpcBase
    {
        private readonly TwitchBot twitchBot;

        public TwitchBotGrpcService(
            TwitchBot twitchBot)
        {
            this.twitchBot = twitchBot;
        }
        public override Task<Empty> JoinChannel(JoinChannelRequest request, ServerCallContext context)
        {
            twitchBot.JoinChannel(request.Channelname);
            return Task.FromResult(new Empty());
        }

        public override async Task<Empty> RefreshChannels(Empty request, ServerCallContext context)
        {
            await twitchBot.RefreshJoinedChannels();
            return new Empty();
        }
    }
}
