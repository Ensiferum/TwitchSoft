using MediatR;
using System.Threading.Tasks;
using TwitchSoft.TelegramBot.MediatR.Models;

namespace TwitchSoft.TelegramBot.TgCommands
{
    public abstract class ParameterizedTgCommand :
        BaseTgCommand
    {
        public abstract string MainParamName { get; }
        protected ParameterizedTgCommand(IMediator mediator) : base(mediator)
        {
        }

        public bool IsValidParameters(params string[] parameters)
        {
            return parameters.Length > 0;
        }

        public async Task RequestAdditionalParameters(string chatId)
        {
            await mediator.Send(new RequestAdditionalInfoCommand
            {
                ChatId = chatId,
                ParamName = MainParamName
            });
        }

        public override string ToString()
        {
            return $"/{BotCommand.ToString().ToLower()} [{MainParamName}] - {Description}";
        }
    }
}
