using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;
using TwitchSoft.Shared.Extensions;
using TwitchSoft.Shared.Services.Models;
using TwitchSoft.TwitchBot.MediatR.Models;
using static TelegramBotGrpc;

namespace TwitchSoft.TwitchBot.MediatR.Handlers
{
    public class NewWhisperMessageHandler : AsyncRequestHandler<NewWhisperMessageDto>
    {
        private readonly TelegramBotGrpcClient telegramBotClient;
        private readonly string rootUserChatId;

        public NewWhisperMessageHandler(TelegramBotGrpcClient telegramBotClient, IConfiguration config)
        {
            this.telegramBotClient = telegramBotClient;

            rootUserChatId = config.GetValue<string>("JobConfigs:RootUserChatId");
        }

        protected override async Task Handle(NewWhisperMessageDto request, CancellationToken cancellationToken)
        {
            var whisperMessage = request.WhisperMessage;

            var messageModel = new ChatMessageModelForDisplaying()
            {
                Channel = "WHISPER MESSAGE",
                Message = whisperMessage.Message,
                PostedTime = DateTime.UtcNow,
                UserName = whisperMessage.Username,
            };

            await telegramBotClient.SendMessageAsync(new SendMessageRequest
            {
                ChatId = rootUserChatId,
                MessageText = messageModel.ToDisplayFormat()
            }, cancellationToken: cancellationToken);
        }
    }
}
