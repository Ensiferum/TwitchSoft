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
        Task<List<ChatMessageModelForDisplaying>> GetMessages(uint userId, int skip = 0, int count = 25);
        Task<List<ChatMessageModelForDisplaying>> GetMessages(uint userId, DateTime from, int count = 25);
        Task RemoveMessagesPriorTo(DateTime dateTime);
        Task<uint> GetUserId(string userName);
        Task CreateOrUpdateUser(User user);
        Task SaveMessagesAsync(params ChatMessage[] chatMessages);
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
