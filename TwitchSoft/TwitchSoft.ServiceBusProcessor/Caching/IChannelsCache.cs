using System.Threading.Tasks;

namespace TwitchSoft.ServiceBusProcessor.Caching
{
    public interface IChannelsCache
    {
        Task<string> GetChannelNameById(uint channelId);
        Task<uint> GetChannelIdByName(string channelName);
    }
}