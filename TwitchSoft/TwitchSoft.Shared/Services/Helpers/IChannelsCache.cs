using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchSoft.Shared.Database.Models;

namespace TwitchSoft.Shared.Services.Helpers
{
    public interface IChannelsCache
    {
        Task<IEnumerable<User>> GetTrackedChannels();
        Task<string> GetChannelNameById(uint channelId);
        Task<uint> GetChannelIdByName(string channelName);
        Task<Dictionary<string, uint>> GetChannelsByNames(params string[] channelNames);
    }
}