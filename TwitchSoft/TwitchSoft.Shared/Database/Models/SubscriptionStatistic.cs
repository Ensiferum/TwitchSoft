using Dapper.Contrib.Extensions;
using System;

namespace TwitchSoft.Shared.Database.Models
{
    [Table("SubscriptionStatistics")]
    public class SubscriptionStatistic
    {
        public uint UserId { get; set; }
        [Write(false)]
        public User User { get; set; }
        public int Count { get; set; }
        public DateTime Date { get; set; }
    }
}
