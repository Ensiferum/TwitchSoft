using MediatR;
using TwitchLib.Client.Models;

namespace TwitchSoft.TwitchBot.MediatR.Models
{
    public record NewUserTimeoutDto : IRequest
    {
        public UserTimeout UserTimeout { get; init; }
    }
}
