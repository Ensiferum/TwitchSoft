using MediatR;

namespace TwitchSoft.TelegramBot.MediatR.Models
{
    public class UnknownCommand : IRequest
    {
        public string ChatId { get; init; }
    }
}
