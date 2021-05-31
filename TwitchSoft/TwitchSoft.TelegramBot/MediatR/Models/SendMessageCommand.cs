using MediatR;

namespace TwitchSoft.TelegramBot.MediatR.Models
{
    public record SendMessageCommand : IRequest
    {
        public string ChatId { get; init; }
        public string MessageText { get; init; }
    }
}
