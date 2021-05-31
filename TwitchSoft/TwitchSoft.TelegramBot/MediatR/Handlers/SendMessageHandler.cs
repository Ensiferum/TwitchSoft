using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TwitchSoft.TelegramBot.MediatR.Models;

namespace TwitchSoft.TelegramBot.MediatR.Handlers
{
    public class SendMessageHandler : AsyncRequestHandler<SendMessageCommand>
    {
        private readonly ITelegramBotClient telegramBotClient;

        public SendMessageHandler(ITelegramBotClient telegramBotClient)
        {
            this.telegramBotClient = telegramBotClient;
        }

        protected override async Task Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            await telegramBotClient.SendTextMessageAsync(
                chatId: request.ChatId,
                text: request.MessageText,
                parseMode: ParseMode.Html,
                disableWebPagePreview: true,
                cancellationToken: cancellationToken
            );
        }
    }
}