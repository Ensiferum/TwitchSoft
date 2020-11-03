using MediatR;
using System.Linq;
using System.Threading.Tasks;
using TwitchSoft.TelegramBot.MediatR.Models;

namespace TwitchSoft.TelegramBot.TgCommands
{
    public class GetSubscribersCountTgCommand :
        ParameterizedTgCommand
    {
        public GetSubscribersCountTgCommand(IMediator mediator) : base(mediator)
        {
        }

        public override string MainParamName => "channel";

        public override BotCommand BotCommand => BotCommand.SubscribersCount;

        public override string Description => "Кол-во сабов для канала";

        public override async Task Execute(string chatId, params string[] parameters)
        {
            var channelName = string.Join(" ", parameters.First());
            await mediator.Send(new SubscribersCountCommand
            {
                ChatId = chatId,
                ChannelName = channelName,
            });
        }
    }
}
