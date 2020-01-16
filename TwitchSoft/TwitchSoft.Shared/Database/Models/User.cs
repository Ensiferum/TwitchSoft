using Dapper.Contrib.Extensions;
using System.Collections.Generic;

namespace TwitchSoft.Shared.Database.Models
{
    [Table("Users")]
    public class User
    {
        [ExplicitKey]
        public uint Id { get; set; }
        public string Username { get; set; }
        public bool JoinChannel { get; set; }
        [Write(false)]
        public List<Subscription> UserSubscriptions { get; set; }
        [Write(false)]
        public List<Subscription> UserSubscriptionGifts { get; set; }
        [Write(false)]
        public List<CommunitySubscription> UserCommunitySubscriptions { get; set; }
        [Write(false)]
        public List<UserBan> UserUserBans { get; set; }
        [Write(false)]
        public List<Subscription> ChannelSubscriptions { get; set; }
        [Write(false)]
        public List<CommunitySubscription> ChannelCommunitySubscriptions { get; set; }
        [Write(false)]
        public List<UserBan> ChannelUserBans { get; set; }
    }
}
