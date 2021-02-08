using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Streams.GetStreams;
using TwitchLib.Api.Helix.Models.Users.GetUserFollows;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using TwitchSoft.Shared.Services.Models.Twitch;

namespace TwitchSoft.Shared.Services.TwitchApi
{
    public class TwitchApiService : ITwitchApiService
    {
        private readonly TwitchAPI api;
        public TwitchApiService(IOptions<BotSettings> options)
        {
            api = new TwitchAPI();
            api.Settings.AccessToken = options.Value.AccessToken;
            api.Settings.ClientId = options.Value.ClientId;
        }

        public async Task<User> GetChannelByName(string channelName)
        {
            var users = await api.Helix.Users.GetUsersAsync(logins: new List<string>() { channelName });
            return users.Users.FirstOrDefault();
        }

        public async Task<IEnumerable<TwitchLib.Api.V5.Models.Channels.Channel>> SearchChannels(string namePart)
        {
            var channels = await api.V5.Search.SearchChannelsAsync(namePart, 10);
            return channels.Channels.OrderByDescending(_ => _.Followers);
        }

        public async Task<List<Follow>> GetFollowsForUser(string fromId, string toId)
        {
            var countToFetch = 100;
            var result = new List<Follow>();
            string cursor = null;
            long total;

            do
            {
                var followsResult = await api.Helix.Users.GetUsersFollowsAsync(cursor, null, countToFetch, fromId, toId);
                total = followsResult.TotalFollows;
                cursor = followsResult.Pagination.Cursor;
                result.AddRange(followsResult.Follows);
            } while (result.Count != total);

            return result;
        }

        public async Task<List<Stream>> GetTopStreams()
        {
            var streamsResponse = await api.Helix.Streams.GetStreamsAsync(first: 100);
            return streamsResponse.Streams.ToList();
        }
    }
}
