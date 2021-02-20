using System;

namespace TwitchSoft.Shared.ElasticSearch.Models
{
    public record ChatMessage
    {
        public Guid Id { get; init; }
        public uint ChannelId { get; init; }
        public string ChannelName { get; init; }
        public string Message { get; init; }
        public DateTime PostedTime { get; init; }
        public bool IsBroadcaster { get; init; }
        public bool IsSubscriber { get; init; }
        public bool IsModerator { get; init; }
        public uint UserId { get; init; }
        public string UserName { get; init; }
    }
}
