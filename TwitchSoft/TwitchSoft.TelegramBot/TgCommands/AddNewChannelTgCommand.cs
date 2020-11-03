using MediatR;
using System.Linq;
using System.Threading.Tasks;
using TwitchSoft.TelegramBot.MediatR.Models;

namespace TwitchSoft.TelegramBot.TgCommands
{
    public class AddNewChannelTgCommand :
        ParameterizedTgCommand
    {
        public AddNewChannelTgCommand(IMediator mediator) : base(mediator)
        {
        }

        public override string MainParamName => "channel";

        public override BotCommand BotCommand => BotCommand.AddChannel;

        public override string Description => "Добавляет канал для отслеживания";

        public override async Task Execute(string chatId, params string[] parameters)
        {
            var channelName = string.Join(" ", parameters.First());
            await mediator.Send(new NewChannelCommand
            {
                ChatId = chatId,
                ChannelName = channelName
            });
        }
    }
}
