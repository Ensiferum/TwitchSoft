using MediatR;
using TwitchLib.Client.Models;

namespace TwitchSoft.TwitchBot.MediatR.Models
{
    public record NewResubscriberDto : IRequest
    {
        public ReSubscriber ReSubscriber { get; init; }
        public string Channel { get; init; }
    }
}
