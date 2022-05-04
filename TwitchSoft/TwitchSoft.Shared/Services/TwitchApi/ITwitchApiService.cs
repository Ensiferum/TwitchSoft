using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchLib.Api.Helix.Models.Search;
using TwitchLib.Api.Helix.Models.Streams.GetStreams;
using TwitchLib.Api.Helix.Models.Users.GetUserFollows;
using TwitchLib.Api.Helix.Models.Users.GetUsers;

namespace TwitchSoft.Shared.Services.TwitchApi
{
    public interface ITwitchApiService
    {
        Task<User> GetChannelByName(string channelName);
        Task<List<Follow>> GetFollowsForUser(string fromId, string toId);
        Task<List<Stream>> GetTopStreams();
        Task<SearchChannelsResponse> SearchChannels(string namePart);
    }
}