using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchSoft.Shared.Models;
using TwitchSoft.Shared.Services.Models;

namespace TwitchSoft.Shared.ElasticSearch.Interfaces
{
    public interface IESService
    {
        Task<List<ChatMessageModelForDisplaying>> GetMessages(uint userId, int skip = 0, int count = 25);
        Task<List<ChatMessageModelForDisplaying>> GetMessages(uint userId, DateTime from, int count = 25);
        Task<List<ChatMessageModelForDisplaying>> SearchMessages(string searchText, int skip, int count);
        Task<IEnumerable<SimpleUserModel>> SearchUsers(string userNamePart, int count = 10);
    }
}
