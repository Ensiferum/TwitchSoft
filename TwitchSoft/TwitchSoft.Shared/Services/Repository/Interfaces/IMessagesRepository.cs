using Nest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchSoft.Shared.ElasticSearch.Models;
using TwitchSoft.Shared.Services.Models;

namespace TwitchSoft.Shared.Services.Repository.Interfaces
{
    public interface IMessagesRepository
    {
        Task<List<ChatMessageModelForDisplaying>> GetMessages(uint userId, int skip = 0, int count = 25);
        Task<List<ChatMessageModelForDisplaying>> GetMessages(uint userId, DateTime from, int count = 25);
        Task<DeleteByQueryResponse> RemoveOldMessages(int days);
        Task<List<ChatMessageModelForDisplaying>> SearchMessages(string searchText, int skip, int count);
        Task SaveMessage(ChatMessage chatMessage);
    }
}
