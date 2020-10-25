using System;
using TwitchSoft.Shared.Database.Models;

namespace TwitchSoft.Shared.ServiceBus.Models
{
    public class NewTwitchChannelMessage
    {
        public Guid Id { get; set; }
        public string Channel { get; set; }
        public string Message { get; set; }
        public DateTime PostedTime { get; set; }
        public User User { get; set; }
        public UserType UserType { get; set; }
        public bool IsBroadcaster { get; set; }
        public bool IsSubscriber { get; set; }
        public bool IsModerator { get; set; }
    }
}
