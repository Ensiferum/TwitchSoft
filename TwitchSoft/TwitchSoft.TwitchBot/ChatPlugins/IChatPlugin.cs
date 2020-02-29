using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace TwitchSoft.TwitchBot.ChatPlugins
{
    public interface IChatPlugin
    {
        Task ProcessMessage(ChatMessage chatMessage, TwitchClient twitchClient);
    }
}
