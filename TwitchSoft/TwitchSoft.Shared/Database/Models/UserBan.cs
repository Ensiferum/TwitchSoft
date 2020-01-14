using Dapper.Contrib.Extensions;
using System;

namespace TwitchSoft.Shared.Database.Models
{
    [Table("UserBans")]
    public class UserBan
    {
        public uint UserId { get; set; }
        public uint ChannelId { get; set; }
        [Write(false)]
        public User Channel { get; set; }
        public string Reason { get; set; }
        public int? Duration { get; set; }
        public DateTime BannedTime { get; set; }
        public BanType BanType { get; set; }
        [Write(false)]
        public User User { get; set; }
    }
}
