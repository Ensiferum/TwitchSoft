using MediatR;
using TwitchLib.Client.Models;

namespace TwitchSoft.TwitchBot.MediatR.Models
{
    public record NewCommunitySubscriptionDto: IRequest
    {
        public CommunitySubscription CommunitySubscription { get; init; }
        public string Channel { get; init; }
    }
}
