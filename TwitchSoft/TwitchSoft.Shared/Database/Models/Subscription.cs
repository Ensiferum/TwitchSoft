using System;

namespace TwitchSoft.Shared.Database.Models
{
    public class Subscription
    {
        public Guid Id { get; set; }
        public uint UserId { get; set; }
        public UserType UserType { get; set; }
        public uint ChannelId { get; set; }
        public User Channel { get; set; }
        public SubscriptionPlan SubscriptionPlan { get; set; }
        public DateTime SubscribedTime { get; set; }
        public int Months { get; set; }
        public User User { get; set; }
        public uint? GiftedBy { get; set; }
        public User GiftedByUser { get; set; }
    }
}
