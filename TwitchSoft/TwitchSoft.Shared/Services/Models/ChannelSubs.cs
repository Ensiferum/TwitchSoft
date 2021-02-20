namespace TwitchSoft.Shared.Services.Models
{
    public record ChannelSubs
    {
        public string Channel { get; init; }
        public int SubsCount { get; init; }
    }
}
