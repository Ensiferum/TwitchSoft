using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Hosting;
using TwitchSoft.TwitchBot.Grpc;

namespace TwitchSoft.TwitchBot
{
    public class TwitchBotGrpcServer : IHostedService
    {
        private readonly TwitchBotGrpcService twitchBotGrpcService;
        private Server server;

        public TwitchBotGrpcServer(TwitchBotGrpcService twitchBotGrpcService)
        {
            this.twitchBotGrpcService = twitchBotGrpcService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            server = new Server
            {
                Services = { TwitchBotGrpc.BindService(twitchBotGrpcService) },
                Ports = { new ServerPort("0.0.0.0", 80, ServerCredentials.Insecure) }
            };
            server.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return server.ShutdownAsync();
        }
    }
}