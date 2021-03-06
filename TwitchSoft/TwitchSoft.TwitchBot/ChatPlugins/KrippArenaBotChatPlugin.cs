﻿using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace TwitchSoft.TwitchBot.ChatPlugins
{
    public class KrippArenaBotChatPlugin : IChatPlugin
    {
        private readonly ILogger<KrippArenaBotChatPlugin> logger;

        public KrippArenaBotChatPlugin(ILogger<KrippArenaBotChatPlugin> logger)
        {
            this.logger = logger;
        }
        public Task ProcessMessage(ChatMessage chatMessage, ITwitchClient twitchClient)
        {
            if (chatMessage.Channel == "nl_kripp" &&
                chatMessage.Username.Equals("streamlabs", StringComparison.OrdinalIgnoreCase) &&
                chatMessage.Message.Equals("BETTING HAS OPENED Pog USE !bet <under | x | over> <1-100> TO BET", StringComparison.OrdinalIgnoreCase))
            {
                logger.LogWarning("Betting started");
                Random rand = new();
                if (rand.Next(3) == 1)
                {
                    logger.LogWarning("Bet under");
                    twitchClient.SendMessage(chatMessage.Channel, "!bet under 100");
                }
            }
            return Task.CompletedTask;
        }
    }
}
