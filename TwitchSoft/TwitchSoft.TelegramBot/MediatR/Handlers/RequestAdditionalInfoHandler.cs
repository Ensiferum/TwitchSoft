using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using TwitchSoft.TelegramBot.MediatR.Models;

namespace TwitchSoft.TelegramBot.MediatR.Handlers
{
    public class RequestAdditionalInfoHandler : AsyncRequestHandler<RequestAdditionalInfoCommand>
    {
        private readonly ITelegramBotClient telegramBotClient;

        public RequestAdditionalInfoHandler(ITelegramBotClient telegramBotClient)
        {
            this.telegramBotClient = telegramBotClient;
        }

        protected override async Task Handle(RequestAdditionalInfoCommand request, CancellationToken cancellationToken)
        {
            await telegramBotClient.SendTextMessageAsync(
                chatId: request.ChatId,
                text: $"Enter parameter [{request.ParamName}] value",
                cancellationToken: cancellationToken
            );
        }
    }
}