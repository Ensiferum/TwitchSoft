using System.Collections.Generic;
using System.Linq;
using TwitchSoft.Shared.Services.Models;
using TwitchSoft.Shared.Extensions;

namespace TwitchSoft.TelegramBot
{
    public static class TelegramBotExtensions
    {
        public const int TELEGRAM_MESSAGE_LIMIT = 4096;
        public static List<string> GenerateReplyMessages(this IEnumerable<ChatMessageModelForDisplaying> messages)
        {
            var messagesFormatted = messages.OrderBy(_ => _.PostedTime)
                    .Select(_ =>
                    {
                        return _.ToDisplayFormat();
                    });

            var replyMessages = new List<string>();
            var replyMessagesTemp = new List<string>();

            foreach (var message in messagesFormatted)
            {
                var prevMessageList = replyMessagesTemp.ToArray();
                replyMessagesTemp.Add(message);
                var generatedMessage = string.Join("\r\n", replyMessagesTemp);
                if (generatedMessage.Length > TELEGRAM_MESSAGE_LIMIT)
                {
                    replyMessages.Add(string.Join("\r\n", prevMessageList));
                    replyMessagesTemp.Clear();
                    replyMessagesTemp.Add(message);
                }
            }
            if (replyMessagesTemp.Any())
            {
                replyMessages.Add(string.Join("\r\n", replyMessagesTemp));
            }

            return replyMessages;
        }
    }
}
