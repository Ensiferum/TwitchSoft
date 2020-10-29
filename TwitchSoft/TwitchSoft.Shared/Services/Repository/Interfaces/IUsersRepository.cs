using System.Collections.Generic;
using System.Threading.Tasks;
using User = TwitchSoft.Shared.Database.Models.User;
using UserTwitch = TwitchLib.Api.Helix.Models.Users.User;

namespace TwitchSoft.Shared.Services.Repository.Interfaces
{
    public interface IUsersRepository
    {
        Task<Dictionary<string, uint>> GetUserIds(params string[] userNames);
        Task CreateOrUpdateUsers(params User[] users);
        Task CreateOrUpdateUser(User user);
        Task<IEnumerable<(uint Id, string Username)>> SearchUsers(string userNamePart, int count = 10);
        Task<IEnumerable<User>> GetChannelsToTrack();
        Task<User> GetUserById(uint id);
        Task<User> GetUserByName(string name);
        Task<bool> AddChannelToTrack(UserTwitch channel);
        Task<IEnumerable<User>> GetUsersByIds(IEnumerable<uint> ids);

        Task SetChannelIsBanned(string channelName, bool isBanned);
    }
}
