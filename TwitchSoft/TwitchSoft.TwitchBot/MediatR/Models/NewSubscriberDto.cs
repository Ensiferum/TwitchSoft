using MediatR;
using TwitchLib.Client.Models;

namespace TwitchSoft.TwitchBot.MediatR.Models
{
    public record NewSubscriberDto : IRequest
    {
        public Subscriber Subscriber { get; init; }
    }
}
