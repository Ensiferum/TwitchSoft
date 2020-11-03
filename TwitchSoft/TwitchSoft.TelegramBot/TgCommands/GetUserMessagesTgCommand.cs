using MediatR;
using System.Linq;
using System.Threading.Tasks;
using TwitchSoft.TelegramBot.MediatR.Models;

namespace TwitchSoft.TelegramBot.TgCommands
{
    public class GetUserMessagesTgCommand :
    ParameterizedTgCommand
    {
        public GetUserMessagesTgCommand(IMediator mediator) : base(mediator)
        {
        }

        public override string MainParamName => "text";

        public override BotCommand BotCommand => BotCommand.UserMessages;

        public override string Description => "Сообщения юзера";

        public override async Task Execute(string chatId, params string[] parameters)
        {
            var userName = parameters.First();
            var skip = 0;
            if (int.TryParse(parameters.ElementAtOrDefault(1), out var skipVal))
            {
                skip = skipVal;
            }

            await mediator.Send(new UserMessagesCommand
            {
                ChatId = chatId,
                Username = userName,
                Skip = skip
            });
        }
    }
}
