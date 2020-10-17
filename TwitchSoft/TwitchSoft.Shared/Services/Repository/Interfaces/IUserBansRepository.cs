using System.Threading.Tasks;
using TwitchSoft.Shared.Database.Models;

namespace TwitchSoft.Shared.Services.Repository.Interfaces
{
    public interface IUserBansRepository
    {
        Task SaveUserBansAsync(params UserBan[] userBans);
    }
}
