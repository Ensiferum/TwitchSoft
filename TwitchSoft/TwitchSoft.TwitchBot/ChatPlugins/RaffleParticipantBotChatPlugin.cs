using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace TwitchSoft.TwitchBot.ChatPlugins
{
    public class RaffleParticipantBotChatPlugin : IChatPlugin
    {
        private readonly ILogger<RaffleParticipantBotChatPlugin> logger;
        private readonly string[] ignoredCommands;

        public RaffleParticipantBotChatPlugin(ILogger<RaffleParticipantBotChatPlugin> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.ignoredCommands = configuration
                .GetValue<string>("ChatPlugins:RaffleParticipantBotChatPlugin:IgnoredCommands")?
                .Split(";", StringSplitOptions.RemoveEmptyEntries).Select(_ => $"!{_}".ToLower()).ToArray();
        }
        public async Task ProcessMessage(ChatMessage chatMessage, ITwitchClient twitchClient)
        {
            if (Regex.IsMatch(chatMessage.Message, "^[!#]\\w+$", RegexOptions.Compiled))
            {
                if (ignoredCommands.Contains(chatMessage.Message.ToLower()))
                {
                    return;
                }

                Random rand = new();
                if (rand.Next(300) == 1)
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
