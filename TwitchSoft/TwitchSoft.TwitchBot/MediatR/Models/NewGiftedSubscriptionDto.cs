using MediatR;
using TwitchLib.Client.Models;

namespace TwitchSoft.TwitchBot.MediatR.Models
{
    public record NewGiftedSubscriptionDto : IRequest
    {
        public GiftedSubscription GiftedSubscription { get; init; }
        public string Channel { get; init; }
    }
}
