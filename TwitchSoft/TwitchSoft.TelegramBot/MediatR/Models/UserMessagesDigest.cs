using MediatR;

namespace TwitchSoft.TelegramBot.MediatR.Models
{
    public class UserMessagesDigest : IRequest
    {
        public string ChatId { get; init; }
        public string Username { get; init; }
    }
}
