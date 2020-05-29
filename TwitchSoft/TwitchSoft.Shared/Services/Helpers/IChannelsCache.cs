using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchSoft.Shared.Database.Models;

namespace TwitchSoft.Shared.Services.Helpers
{
    public interface IChannelsCache
    {
        Task<string> GetChannelNameById(uint channelId);
        Task<uint> GetChannelIdByName(string channelName);
    }
}