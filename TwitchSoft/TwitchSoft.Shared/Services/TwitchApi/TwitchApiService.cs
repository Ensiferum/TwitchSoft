using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Streams;
using TwitchLib.Api.Helix.Models.Users;
using TwitchSoft.Shared.Services.Models.Twitch;

namespace TwitchSoft.Shared.Services.TwitchApi
{
    public class TwitchApiService : ITwitchApiService
    {
        private TwitchAPI api;
        public TwitchApiService(IOptions<BotSettings> options)
        {
            api = new TwitchAPI();
            api.Settings.AccessToken = options.Value.BotOAuthToken.Split(':')[1];
        }

        public async Task<User> GetChannelByName(string channelName)
        {
            var users = await api.Helix.Users.GetUsersAsync(logins: new List<string>() { channelName });
            return users.Users.FirstOrDefault();
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
