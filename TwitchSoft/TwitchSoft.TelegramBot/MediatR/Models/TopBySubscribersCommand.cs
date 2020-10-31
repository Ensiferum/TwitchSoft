using MediatR;

namespace TwitchSoft.TelegramBot.MediatR.Models
{
    public class TopBySubscribersCommand : IRequest
    {
        public string ChatId { get; init; }
        public string SkipString { get; init; }
    }
}
