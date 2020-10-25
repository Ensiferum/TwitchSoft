using System.Threading.Tasks;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace TwitchSoft.TwitchBot.ChatPlugins
{
    public interface IChatPlugin
    {
        Task ProcessMessage(ChatMessage chatMessage, ITwitchClient twitchClient);
    }
}
