using MediatR;

namespace TwitchSoft.TelegramBot.MediatR.Models
{
    public class TopSubscribersCommand : IRequest
    {
        public string ChatId { get; init; }
        public string ParamString { get; init; }
    }
}
