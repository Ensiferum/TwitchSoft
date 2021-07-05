using MediatR;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;
using TwitchSoft.ServiceBusProcessor.MediatR.Models;
using TwitchSoft.Shared;
using TwitchSoft.Shared.Extensions;
using TwitchSoft.Shared.Services.Models;
using static TelegramBotGrpc;

namespace TwitchSoft.ServiceBusProcessor.MediatR.Handlers
{
    public class SendTelegramMessageHandler : AsyncRequestHandler<SendTelegramMessage>
    {
        private readonly TelegramBotGrpcClient telegramBotClient;

        public SendTelegramMessageHandler(TelegramBotGrpcClient telegramBotClient, IConfiguration config)
        {
            this.telegramBotClient = telegramBotClient;

            rootUserChatId = config.GetValue<string>("JobConfigs:RootUserChatId");
        }

        private readonly string rootUserChatId;

        protected override async Task Handle(SendTelegramMessage request, CancellationToken cancellationToken)
        {
            var chatMessage = request.Message;

            if (chatMessage.User.UserId == Constants.MadTwitchId ||
                chatMessage.Message.Contains($"@{Constants.EnsthorTwitchName}", System.StringComparison.OrdinalIgnoreCase))
            {
                var messageModel = new ChatMessageModelForDisplaying()
                {
                    Channel = chatMessage.Channel,
                    Message = chatMessage.Message,
                    PostedTime = chatMessage.PostedTime,
                    UserName = chatMessage.User.UserName,
                };

                await telegramBotClient.SendMessageAsync(new SendMessageRequest
                {
                    ChatId = rootUserChatId,
                    MessageText = messageModel.ToDisplayFormat()
                }, cancellationToken: cancellationToken);
            }
        }
    }
}
