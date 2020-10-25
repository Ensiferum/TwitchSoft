using System;
using TwitchSoft.Shared.Database.Models;

namespace TwitchSoft.Shared.ServiceBus.Models
{
    public class NewCommunitySubscription
    {
        public Guid Id { get; set; }
        public string Channel { get; set; }
        public DateTime Date { get; set; }
        public int GiftCount { get; set; }
        public SubscriptionPlan SubscriptionPlan { get; set; }
        public User User { get; set; }
    }
}
