using System;
using TwitchSoft.Shared.Database.Models;

namespace TwitchSoft.Shared.ServiceBus.Models
{
    public class NewBan
    {
        public User Channel { get; set; }
        public string Reason { get; set; }
        public int? Duration { get; set; }
        public DateTime BannedTime { get; set; }
        public BanType BanType { get; set; }
        public User User { get; set; }
    }
}
