﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchSoft.Shared.Models;
using User = TwitchSoft.Shared.Database.Models.User;
using UserTwitch = TwitchLib.Api.Helix.Models.Users.GetUsers.User;

namespace TwitchSoft.Shared.Services.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<Dictionary<string, uint>> GetUserIds(params string[] userNames);
        Task CreateOrUpdateUsers(params User[] users);
        Task CreateOrUpdateUser(User user);
        Task<IEnumerable<SimpleUserModel>> SearchUsers(string userNamePart, int count = 10);
        Task<IEnumerable<User>> GetChannelsToTrack();
        Task<User> GetUserById(uint id);
        Task<User> GetUserByName(string name);
        Task<bool> AddChannelToTrack(UserTwitch channel);
        Task<IEnumerable<User>> GetUsersByIds(IEnumerable<uint> ids);
        Task<IEnumerable<User>> GetBannedChannels();
        Task SetChannelIsBanned(string channelName, bool isBanned);
        Task SetChannelIsBanned(uint userId, bool isBanned);
    }
}
