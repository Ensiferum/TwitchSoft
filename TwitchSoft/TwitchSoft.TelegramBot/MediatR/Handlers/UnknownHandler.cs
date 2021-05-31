using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using TwitchSoft.TelegramBot.MediatR.Models;
using TwitchSoft.TelegramBot.TgCommands;

namespace TwitchSoft.TelegramBot.MediatR.Handlers
{
    public class UnknownHandler : AsyncRequestHandler<UnknownCommand>
    {
        private readonly ITelegramBotClient telegramBotClient;
        private readonly IEnumerable<BaseTgCommand> tgCommands;

        public UnknownHandler(
            ITelegramBotClient telegramBotClient, 
            IEnumerable<BaseTgCommand> tgCommands)
        {
            this.telegramBotClient = telegramBotClient;
            this.tgCommands = tgCommands;
        }

        protected override async Task Handle(UnknownCommand request, CancellationToken cancellationToken)
        {
            string usage = $@"
Usage:
{string.Join("\n", tgCommands.Select(com => com.ToString()))}";

            await telegramBotClient.SendTextMessageAsync(
                request.ChatId,
                usage,
                cancellationToken: cancellationToken);
        }
    }
}