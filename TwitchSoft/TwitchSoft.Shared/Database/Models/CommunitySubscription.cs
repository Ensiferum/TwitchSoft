using Dapper.Contrib.Extensions;
using System;

namespace TwitchSoft.Shared.Database.Models
{
    [Table("CommunitySubscriptions")]
    public class CommunitySubscription
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        public uint ChannelId { get; set; }
        [Write(false)]
        public User Channel { get; set; }
        public uint UserId { get; set; }
        public DateTime Date { get; set; }
        public int GiftCount { get; set; }
        public SubscriptionPlan SubscriptionPlan { get; set; }
        [Write(false)]
        public User User { get; set; }
    }
}
