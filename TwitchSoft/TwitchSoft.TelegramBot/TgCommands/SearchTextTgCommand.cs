using MediatR;
using System.Linq;
using System.Threading.Tasks;
using TwitchSoft.TelegramBot.MediatR.Models;

namespace TwitchSoft.TelegramBot.TgCommands
{
    public class SearchTextTgCommand :
        ParameterizedTgCommand
    {
        public SearchTextTgCommand(IMediator mediator) : base(mediator)
        {
        }

        public override string MainParamName => "text";

        public override BotCommand BotCommand => BotCommand.SearchText;

        public override string Description => "Поиск по тексту сообщений";

        public override async Task Execute(string chatId, params string[] parameters)
        {
            var searchText = parameters.First();
            var skip = 0;
            if (int.TryParse(parameters.ElementAtOrDefault(1), out var skipVal))
            {
                skip = skipVal;
            }

            await mediator.Send(new SearchTextCommand
            {
                ChatId = chatId,
                SearchText = searchText,
                Skip = skip
            });
        }
    }
}
