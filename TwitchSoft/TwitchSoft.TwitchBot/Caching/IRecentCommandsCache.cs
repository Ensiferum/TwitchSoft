using System.Threading.Tasks;

namespace TwitchSoft.TwitchBot.Caching
{
    public interface IRecentCommandsCache
    {
        int GetAndUpdateCommandOccurences(string command);
        void DeleteCommand(string command);
    }
}