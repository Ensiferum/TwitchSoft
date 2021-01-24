using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchLib.Api.Helix.Models.Streams;
using TwitchLib.Api.Helix.Models.Users;
using TwitchLib.Api.V5.Models.Channels;

namespace TwitchSoft.Shared.Services.TwitchApi
{
    public interface ITwitchApiService
    {
        Task<User> GetChannelByName(string channelName);
        Task<List<Follow>> GetFollowsForUser(string fromId, string toId);
        Task<List<Stream>> GetTopStreams();
        Task<Channel[]> SearchChannels(string namePart);
    }
}