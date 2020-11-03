using MediatR;
using System.Threading.Tasks;

namespace TwitchSoft.TelegramBot.TgCommands
{
    public abstract class BaseTgCommand
    {
        internal readonly IMediator mediator;
        public BaseTgCommand(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public abstract string Description { get; }
        public abstract BotCommand BotCommand { get; }
        public abstract Task Execute(string chatId, params string[] parameters);

        public override string ToString()
        {
            return $"/{BotCommand.ToString().ToLower()} - {Description}";
        }
    }
}
