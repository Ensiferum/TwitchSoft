using Dapper.Contrib.Extensions;
using System;

namespace TwitchSoft.Shared.Database.Models
{
    [Table("Subscriptions")]
    public class Subscription
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        public uint UserId { get; set; }
        [Write(false)]
        public User User { get; set; }
        public UserType UserType { get; set; }
        public uint ChannelId { get; set; }
        [Write(false)]
        public User Channel { get; set; }
        public SubscriptionPlan SubscriptionPlan { get; set; }
        public DateTime SubscribedTime { get; set; }
        public int Months { get; set; }
        
        public uint? GiftedBy { get; set; }
        [Write(false)]
        public User GiftedByUser { get; set; }
    }
}
