using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;

namespace TwitchSoft.TelegramBot
{
    public static class Utils
    {
        public static InlineKeyboardMarkup GenerateNavigationMarkup(string command, string parameter, int itemsLimitCount, int skip, int itemsCount)
        {
            var inlineButtons = new List<InlineKeyboardButton>();
            if (itemsCount > 0)
            {
                if (skip > 0)
                {
                    inlineButtons.Add(
                        InlineKeyboardButton
                        .WithCallbackData($"Prev {itemsLimitCount}", $"{command} {parameter} {(skip - itemsLimitCount > 0 ? skip - itemsLimitCount : 0)}")
                    );
                }
                if (itemsCount == itemsLimitCount)
                {
                    inlineButtons.Add(
                        InlineKeyboardButton
                        .WithCallbackData($"Next {itemsLimitCount}", $"{command} {parameter} {skip + itemsLimitCount}")
                    );
                }

                if (skip >= itemsLimitCount)
                {
                    inlineButtons.Add(InlineKeyboardButton
                    .WithCallbackData($"Reset", $"{command} {parameter}"));
                }
                else
                {
                    inlineButtons.Add(InlineKeyboardButton
                    .WithCallbackData($"Refresh", $"{command} {parameter}"));
                }
            }

            return new InlineKeyboardMarkup(new[]
                    {
                        inlineButtons
                    });
        }
    }
}
