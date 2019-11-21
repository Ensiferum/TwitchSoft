using System;

namespace TwitchSoft.Shared.Database.Models
{
    public class UserBan
    {
        public uint UserId { get; set; }
        public uint ChannelId { get; set; }
        public User Channel { get; set; }
        public string Reason { get; set; }
        public int? Duration { get; set; }
        public DateTime BannedTime { get; set; }
        public BanType BanType { get; set; }
        public User User { get; set; }
    }
}
