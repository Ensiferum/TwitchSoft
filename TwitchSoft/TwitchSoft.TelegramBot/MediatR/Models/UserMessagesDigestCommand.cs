using MediatR;

namespace TwitchSoft.TelegramBot.MediatR.Models
{
    public class UserMessagesDigestCommand : IRequest
    {
        public string ChatId { get; init; }
        public string UserName { get; init; }
        public uint? TwitchUserId { get; init; }
    }
}
