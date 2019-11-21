using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace TwitchSoft.TwitchBot.ChatPlugins
{
    public interface IChatPlugin
    {
        void ProcessMessage(ChatMessage chatMessage, TwitchClient twitchClient);
    }
}
