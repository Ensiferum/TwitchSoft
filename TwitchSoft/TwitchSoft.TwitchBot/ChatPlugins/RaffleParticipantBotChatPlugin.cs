using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace TwitchSoft.TwitchBot.ChatPlugins
{
    public class RaffleParticipantBotChatPlugin : IChatPlugin
    {
        private readonly ILogger<RaffleParticipantBotChatPlugin> logger;

        private static readonly ReadOnlyCollection<string> IgnoreCommands = new ReadOnlyCollection<string> (
            new string[] {
                "!discord", 
                "!buy", 
                "!love",
                "!comands",
                "!lettuce",
                "!uptime",
                "!yes",
                "!trails",
                "!binds",
                "!modpack",
                "!settings",
                "!akm",
                "!followage",
                "!followerage",
                "!follwage",
                "!specs",
                "!help",
                "!twitter",
                "!vote",
                "!mods",
                "!play",
                "!leave",
                "!сборки",
                "!стрим",
                "!любовь",
                "!лагуны",
                "!m4",
                "!бочка",
                "!sa",
                "!пс",
                "!rtx",
                "!сан",
                "!тргг",
                "!rgg",
                "!song",
                "!poll",
                "!трек",
                "!game",
                "!гав",
                "!fs",
                "!val",
                "!hk",
                "!goty",
                "!up",
                "!sub",
                "!магазин",
                "!настройки",
                "!ak"
            }
        );

        public RaffleParticipantBotChatPlugin(ILogger<RaffleParticipantBotChatPlugin> logger)
        {
            this.logger = logger;
        }
        public async Task ProcessMessage(ChatMessage chatMessage, TwitchClient twitchClient)
        {
            if (Regex.IsMatch(chatMessage.Message, "^[!#]\\w+$", RegexOptions.Compiled))
            {
                if (IgnoreCommands.Contains(chatMessage.Message.ToLower()))
                {
                    return;
                }

                Random rand = new Random();
                if (rand.Next(100) == 1)
                {
                    logger.LogWarning($"Participate in raffle on channel {chatMessage.Channel} with command {chatMessage.Message}");
                    await Task.Run(async () =>
                    {
                        await Task.Delay(rand.Next(5000));
                        twitchClient.SendMessage(chatMessage.Channel, chatMessage.Message);
                    });
                }
            }
        }
    }
}
