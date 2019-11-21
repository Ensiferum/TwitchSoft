using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchSoft.Shared.Database.Models;

namespace TwitchSoft.Shared.Services.Helpers
{
    public interface IChannelsCache
    {
        Task<List<User>> GetTrackedChannels();
        Task<User> GetChannelById(uint channelId);
        Task<User> GetChannelByName(string channelName);
        void InvalidateCache();
    }
}