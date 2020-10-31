using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using TwitchSoft.TelegramBot.MediatR.Models;

namespace TwitchSoft.TelegramBot.MediatR.Handlers
{
    public class UnknownCommandHandler : AsyncRequestHandler<UnknownCommand>
    {
        private readonly ITelegramBotClient telegramBotClient;

        public UnknownCommandHandler(ITelegramBotClient telegramBotClient)
        {
            this.telegramBotClient = telegramBotClient;
        }

        protected override async Task Handle(UnknownCommand request, CancellationToken cancellationToken)
        {
            string usage = $@"
Usage:
{BotCommands.UserMessages} [username] - покажет сообщения для пользователя
{BotCommands.AddChannel} [channel] - добавляет канал для отслеживания
{BotCommands.SubscribersCount} - выводит топ 100 каналов по кол-ву сабов
{BotCommands.SearchText} [text] - поиск по тексту сообщений";

            await telegramBotClient.SendTextMessageAsync(
                request.ChatId,
                usage,
                cancellationToken: cancellationToken);
        }
    }
}
