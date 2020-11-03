using MediatR;

namespace TwitchSoft.TelegramBot.MediatR.Models
{
    public class UserMessagesCommand : IRequest
    {
        public string ChatId { get; init; }
        public string Username { get; init; }
        public int Skip { get; init; }
    }
}
