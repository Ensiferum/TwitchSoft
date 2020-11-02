using MediatR;

namespace TwitchSoft.TelegramBot.MediatR.Models
{
    public class RequestAdditionalInfoCommand : IRequest
    {
        public string ChatId { get; init; }
        public string ParamName { get; init; }
    }
}
