using Dapper.Contrib.Extensions;
using System;

namespace TwitchSoft.Shared.Database.Models
{
    [Table("UserBans")]
    public class UserBan
    {
        [Key]
        public long Id { get; set; }
        public string Channel { get; set; }
        public string UserName { get; set; }
        public string Reason { get; set; }
        public int? Duration { get; set; }
        public DateTime BannedTime { get; set; }
        public BanType BanType { get; set; }
    }
}
