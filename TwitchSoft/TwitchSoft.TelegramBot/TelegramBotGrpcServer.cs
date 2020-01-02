using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Hosting;
using TwitchSoft.TelegramBot.Grpc;

namespace TwitchSoft.TelegramBot
{
    public class TelegramBotGrpcServer : IHostedService
    {
        private readonly TelegramBotGrpcService telegramBotGrpcService;
        private Server server;

        public TelegramBotGrpcServer(TelegramBotGrpcService telegramBotGrpcService)
        {
            this.telegramBotGrpcService = telegramBotGrpcService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            server = new Server
            {
                Services = { TelegramBotGrpc.BindService(telegramBotGrpcService) },
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