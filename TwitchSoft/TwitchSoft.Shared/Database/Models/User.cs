using System.Collections.Generic;

namespace TwitchSoft.Shared.Database.Models
{
    public class User
    {
        public uint Id { get; set; }
        public string Username { get; set; }
        public bool JoinChannel { get; set; }
        public bool TrackMessages { get; set; }

        public List<ChatMessage> UserChatMessages { get; set; }
        public List<Subscription> UserSubscriptions { get; set; }
        public List<Subscription> UserSubscriptionGifts { get; set; }
        public List<CommunitySubscription> UserCommunitySubscriptions { get; set; }
        public List<UserBan> UserUserBans { get; set; }

        public List<ChatMessage> ChannelChatMessages { get; set; }
        public List<Subscription> ChannelSubscriptions { get; set; }
        public List<CommunitySubscription> ChannelCommunitySubscriptions { get; set; }
        public List<UserBan> ChannelUserBans { get; set; }
    }
}
