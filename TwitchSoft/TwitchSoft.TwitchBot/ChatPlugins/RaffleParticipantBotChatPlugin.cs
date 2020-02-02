using Microsoft.Extensions.Logging;
using System;
using System.Text.RegularExpressions;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace TwitchSoft.TwitchBot.ChatPlugins
{
    public class RaffleParticipantBotChatPlugin : IChatPlugin
    {
        private readonly ILogger<RaffleParticipantBotChatPlugin> logger;

        public RaffleParticipantBotChatPlugin(ILogger<RaffleParticipantBotChatPlugin> logger)
        {
            this.logger = logger;
        }
        public void ProcessMessage(ChatMessage chatMessage, TwitchClient twitchClient)
        {
            if (Regex.IsMatch(chatMessage.Message, "^!\\w+$", RegexOptions.Compiled))
            {
                Random rand = new Random();
                if (rand.Next(50) == 1)
                {
                    logger.LogWarning($"Participate in raffle on channel {chatMessage.Channel} with command {chatMessage.Message}");
                    twitchClient.SendMessage(chatMessage.Channel, chatMessage.Message);
                }
            }
        }
    }
}
