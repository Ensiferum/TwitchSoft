using System;
using TwitchSoft.Shared.Database.Models;

namespace TwitchSoft.Shared.ServiceBus.Models
{
    public record NewSubscriber
    {
        public Guid Id { get; set; }
        public UserType UserType { get; set; }
        public string Channel { get; set; }
        public SubscriptionPlan SubscriptionPlan { get; set; }
        public DateTime SubscribedTime { get; set; }
        public int Months { get; set; }
        public User GiftedBy { get; set; }
        public User User { get; set; }
    }
}
