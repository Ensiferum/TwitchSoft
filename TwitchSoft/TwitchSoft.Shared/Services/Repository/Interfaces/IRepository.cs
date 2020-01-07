using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchSoft.Shared.Database.Models;
using TwitchSoft.Shared.Services.Models;
using User = TwitchSoft.Shared.Database.Models.User;
using UserTwitch = TwitchLib.Api.Helix.Models.Users.User;

namespace TwitchSoft.Shared.Services.Repository.Interfaces
{
    public interface IRepository
    {
        Task<uint> GetUserId(string userName);
        Task CreateOrUpdateUser(User user);
        Task<List<User>> SearchUsers(string userNamePart, int count = 10);
        Task SaveSubscriberAsync(params Subscription[] subscription);
        Task SaveCommunitySubscribtionAsync(CommunitySubscription communitySubscription);
        Task SaveUserBanAsync(UserBan userBan);
        Task<List<User>> GetChannelsToTrack();
        Task<bool> AddChannelToTrack(UserTwitch channel);
        Task<List<ChannelSubs>> GetTopChannelsBySubscribers(int skip, int count);
        Task<int> GetSubscribersCountFor(string channel);
    }
}
