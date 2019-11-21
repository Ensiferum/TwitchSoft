using System;

namespace TwitchSoft.Shared.Database.Models
{
    public class ChatMessage
    {
        public Guid Id { get; set; }
        public uint ChannelId { get; set; }
        public User Channel { get; set; }
        public string Message { get; set; }
        public DateTime PostedTime { get; set; }
        public bool IsBroadcaster { get; set; }
        public bool IsSubscriber { get; set; }
        public bool IsModerator { get; set; }
        public uint UserId { get; set; }
        public UserType UserType { get; set; }
        public User User { get; set; }
    }
}
