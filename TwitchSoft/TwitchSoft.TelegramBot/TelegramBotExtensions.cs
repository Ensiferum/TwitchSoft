using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TwitchSoft.Shared.Services.Models;

namespace TwitchSoft.TelegramBot
{
    public static class TelegramBotExtensions
    {
        public static List<string> GenerateReplyMessages(this IEnumerable<ChatMessageModelForDisplaying> messages)
        {
            var messagesFormatted = messages.OrderBy(_ => _.PostedTime)
                    .Select(_ =>
                    {
                        var dateFormat = _.PostedTime.Year == DateTime.Today.Year ? "dd/MM HH:mm:ss" : "dd/MM/yyyy HH:mm:ss";
                        if (_.PostedTime.Date == DateTime.Today)
                        {
                            dateFormat = "HH:mm:ss";
                        }
                        return $@"
{_.PostedTime.ToString(dateFormat)}: Channel: <b>{_.Channel}</b>
<b>{_.UserName}</b>: {HttpUtility.HtmlEncode(_.Message)}";
                    });

            var replyMessages = new List<string>();
            var replyMessagesTemp = new List<string>();

            foreach (var message in messagesFormatted)
            {
                var prevMessageList = replyMessagesTemp.ToArray();
                replyMessagesTemp.Add(message);
                var generatedMessage = string.Join("\r\n", replyMessagesTemp);
                if (generatedMessage.Length > TelegramBotCommandProcessor.TELEGRAM_MESSAGE_LIMIT)
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
