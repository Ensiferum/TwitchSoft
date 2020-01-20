using Microsoft.Extensions.Logging;
using System;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace TwitchSoft.TwitchBot.ChatPlugins
{
    public class MaincastBotChatPlugin : IChatPlugin
    {
        private readonly ILogger<MaincastBotChatPlugin> logger;

        public MaincastBotChatPlugin(ILogger<MaincastBotChatPlugin> logger)
        {
            this.logger = logger;
        }
        public void ProcessMessage(ChatMessage chatMessage, TwitchClient twitchClient)
        {
            if (chatMessage.Channel == "dota2mc_ru" &&
                chatMessage.Message.Equals("!maincast", StringComparison.OrdinalIgnoreCase))
            {
                Random rand = new Random();
                if (rand.Next(50) == 1)
                {
                    logger.LogWarning("Maincast participate !maincast");
                    twitchClient.SendMessage(chatMessage.Channel, "!maincast");
                }
            }
        }
    }
}
