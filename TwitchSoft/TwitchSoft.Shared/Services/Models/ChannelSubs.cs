namespace TwitchSoft.Shared.Services.Models
{
    public record ChannelSubs
    {
        public string Channel { get; set; }
        public int SubsCount { get; set; }
    }
}
