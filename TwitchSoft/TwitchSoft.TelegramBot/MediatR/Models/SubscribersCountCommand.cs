using MediatR;

namespace TwitchSoft.TelegramBot.MediatR.Models
{
    public class SubscribersCountCommand : IRequest
    {
        public string ChatId { get; init; }
        public string ChannelName { get; init; }
    }
}
