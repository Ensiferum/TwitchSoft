using System;

namespace TwitchSoft.Shared.Database.Models
{
    public class CommunitySubscription
    {
        public Guid Id { get; set; }
        public uint ChannelId { get; set; }
        public User Channel { get; set; }
        public uint UserId { get; set; }
        public DateTime Date { get; set; }
        public int GiftCount { get; set; }
        public SubscriptionPlan SubscriptionPlan { get; set; }
        public User User { get; set; }
    }
}
