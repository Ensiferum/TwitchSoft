using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchSoft.TwitchBot.Caching;

namespace TwitchSoft.TwitchBot.ChatPlugins
{
    public class RaffleParticipantBotChatPlugin : IChatPlugin
    {
        private const int NumberOfOccurencesToTrigger = 30;
        private const int RandomizerChance = 25;
        private readonly ILogger<RaffleParticipantBotChatPlugin> logger;
        private readonly IRecentCommandsCache recentCommandsCache;
        private readonly string[] ignoredCommands;

        public RaffleParticipantBotChatPlugin(
            ILogger<RaffleParticipantBotChatPlugin> logger,
            IConfiguration configuration,
            IRecentCommandsCache recentCommandsCache)
        {
            this.logger = logger;
            this.recentCommandsCache = recentCommandsCache;
            this.ignoredCommands = configuration
                .GetValue<string>("ChatPlugins:RaffleParticipantBotChatPlugin:IgnoredCommands")?
                .Split(";", StringSplitOptions.RemoveEmptyEntries).Select(_ => $"!{_}".ToLower()).ToArray();
        }
        public async Task ProcessMessage(ChatMessage chatMessage, ITwitchClient twitchClient)
        {
            var message = chatMessage.Message;
            if (Regex.IsMatch(message, @"^[!#]\w+$", RegexOptions.Compiled) && Regex.IsMatch(message, @"^\P{IsCyrillic}+$", RegexOptions.Compiled))
            {

                if (ignoredCommands.Contains(message, StringComparer.OrdinalIgnoreCase))
                {
                    return;
                }

                Random rand = new();
                if (rand.Next(100) <= RandomizerChance)
                {
                    var occurenceNumber = recentCommandsCache.GetAndUpdateCommandOccurences(message);

                    if (occurenceNumber >= NumberOfOccurencesToTrigger)
                    {
                        recentCommandsCache.DeleteCommand(message);

                        logger.LogWarning($"Participate in raffle on channel {chatMessage.Channel} with command {message}");
                        await Task.Run(async () =>
                        {
                            await Task.Delay(rand.Next(5000));
                            twitchClient.SendMessage(chatMessage.Channel, message);
                        });
                    }
                }
            }
        }
    }
}
