using MediatR;

namespace TwitchSoft.TelegramBot.MediatR.Models
{
    public class SearchTextCommand : IRequest
    {
        public string ChatId { get; init; }
        public string SearchText { get; init; }
        public string SkipString { get; init; }
    }
}
