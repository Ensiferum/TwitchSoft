using System.Threading.Tasks;

namespace TwitchSoft.Shared.Services.Helpers
{
    public interface IChannelsCache
    {
        Task<string> GetChannelNameById(uint channelId);
        Task<uint> GetChannelIdByName(string channelName);
    }
}