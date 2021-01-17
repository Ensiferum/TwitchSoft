using MediatR;
using System.Linq;
using System.Threading.Tasks;
using TwitchSoft.TelegramBot.MediatR.Models;

namespace TwitchSoft.TelegramBot.TgCommands
{
    public class DailyNewSubscribersCountTgCommand :
        BaseTgCommand
    {
        public DailyNewSubscribersCountTgCommand(IMediator mediator) : base(mediator)
        {

        }
        public override BotCommand BotCommand => BotCommand.NewSubscribersCountDaily;

        public override string Description => "Кол-во новых сабов за день по каналам";

        public override async Task Execute(string chatId, params string[] parameters)
        {
            var skip = 0;
            if (int.TryParse(parameters.FirstOrDefault(), out var skipVal))
            {
                skip = skipVal;
            }

            await mediator.Send(new TopBySubscribersCommand
            {
                ChatId = chatId,
                Skip = skip,
            });
        }
    }
}
