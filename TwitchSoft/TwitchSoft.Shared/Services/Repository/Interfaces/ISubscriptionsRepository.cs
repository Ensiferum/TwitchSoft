using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchSoft.Shared.Database.Models;
using TwitchSoft.Shared.Services.Models;

namespace TwitchSoft.Shared.Services.Repository.Interfaces
{
    public interface ISubscriptionsRepository
    {
        Task SaveSubscriberAsync(params Subscription[] subscription);
        Task SaveCommunitySubscribtionAsync(CommunitySubscription communitySubscription);
        Task<IEnumerable<ChannelSubs>> GetSubscribersCount(int skip, int count, DateTime? fromDate = null);
        Task<int> GetSubscribersCountFor(string channel);
    }
}
