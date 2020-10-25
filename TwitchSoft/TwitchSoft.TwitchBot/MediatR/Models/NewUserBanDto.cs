using MediatR;
using TwitchLib.Client.Models;

namespace TwitchSoft.TwitchBot.MediatR.Models
{
    public record NewUserBanDto : IRequest
    {
        public UserBan UserBan { get; init; }
    }
}
