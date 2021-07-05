using MediatR;
using TwitchSoft.Shared.ServiceBus.Models;

namespace TwitchSoft.ServiceBusProcessor.MediatR.Models
{
    public record SendTelegramMessage: IRequest
    {
        public NewTwitchChannelMessage Message { get; init; }
    }
}
