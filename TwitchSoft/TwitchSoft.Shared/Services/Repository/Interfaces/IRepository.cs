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
        Task<Dictionary<string, uint>> GetUserIds(params string[] userNames);
        Task CreateOrUpdateUsers(params User[] users);
        Task CreateOrUpdateUser(User user);
        Task<IEnumerable<(uint Id, string Username)>> SearchUsers(string userNamePart, int count = 10);
        Task SaveSubscriberAsync(params Subscription[] subscription);
        Task SaveCommunitySubscribtionAsync(CommunitySubscription communitySubscription);
        Task SaveUserBansAsync(params UserBan[] userBans);
        Task<IEnumerable<User>> GetChannelsToTrack();
        Task<bool> AddChannelToTrack(UserTwitch channel);
        Task<IEnumerable<ChannelSubs>> GetTopChannelsBySubscribers(int skip, int count);
        Task<int> GetSubscribersCountFor(string channel);
        Task<IEnumerable<User>> GetUsersByIds(IEnumerable<uint> ids);
    }
}
