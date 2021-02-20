using System.Threading.Tasks;
using TwitchSoft.Shared.Database.Models;

namespace TwitchSoft.Shared.Services.Repository.Interfaces
{
    public interface IUserBanRepository
    {
        Task SaveUserBansAsync(params UserBan[] userBans);
    }
}
