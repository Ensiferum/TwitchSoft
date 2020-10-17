using System;

namespace TwitchSoft.Shared.ElasticSearch.Models
{
    public record ChatMessage
    {
        public Guid Id { get; set; }
        public uint ChannelId { get; set; }
        public string ChannelName { get; set; }
        public string Message { get; set; }
        public DateTime PostedTime { get; set; }
        public bool IsBroadcaster { get; set; }
        public bool IsSubscriber { get; set; }
        public bool IsModerator { get; set; }
        public uint UserId { get; set; }
        public string UserName { get; set; }
    }
}
