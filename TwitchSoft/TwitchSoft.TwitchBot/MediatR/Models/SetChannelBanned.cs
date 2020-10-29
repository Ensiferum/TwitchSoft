using MediatR;

namespace TwitchSoft.TwitchBot.MediatR.Models
{
    public record SetChannelBanned : IRequest
    {
        public string Channel { get; init; }
        public bool IsBanned { get; init; }
    }
}
