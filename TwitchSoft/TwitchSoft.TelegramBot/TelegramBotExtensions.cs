using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TwitchSoft.Shared.Services.Models;
using TwitchSoft.Shared.Services.Helpers;

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
                        var postedTime = _.PostedTime.ConvertToMyTimezone();
                        var dateFormat = postedTime.Year == postedTime.Year ? "dd/MM HH:mm:ss" : "dd/MM/yyyy HH:mm:ss";
                        if (postedTime.Date == DateTime.Today)
                        {
                            dateFormat = "HH:mm:ss";
                        }
                        return $@"
{postedTime.ToString(dateFormat)}: Channel: <b>{_.Channel}</b>
<b>{_.UserName}</b>: {HttpUtility.HtmlEncode(_.Message)}";
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
