using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using TwitchSoft.Shared.Services.TwitchApi;
using TwitchSoft.TelegramBot.MediatR.Models;

namespace TwitchSoft.TelegramBot.MediatR.Handlers
{
    public class InlineUsersSearchHandler : AsyncRequestHandler<InlineUsersSearchCommand>
    {
        private readonly ITwitchApiService twitchApiService;
        private readonly ITelegramBotClient telegramBotClient;

        public InlineUsersSearchHandler(
            ITwitchApiService twitchApiService,
            ITelegramBotClient telegramBotClient)
        {
            this.twitchApiService = twitchApiService;
            this.telegramBotClient = telegramBotClient;
        }

        protected override async Task Handle(InlineUsersSearchCommand request, CancellationToken cancellationToken)
        {
            var searchUserText = request.SearchUserText.ToLower();
            var channels = await twitchApiService.SearchChannels(searchUserText);

            var results = new List<InlineQueryResultArticle>();
            foreach (var user in channels)
            {
                var article = new InlineQueryResultArticle(
                    id: user.Id,
                    title: user.DisplayName,
                    inputMessageContent: new InputTextMessageContent(user.DisplayName))
                {
                    ReplyMarkup = new InlineKeyboardMarkup(new[]
                        {
                                new[] {
                                    InlineKeyboardButton.WithCallbackData(
                                        "Show messages",
                                        $"{BotCommands.UserMessages} {user.Name}")
                                },
                                new[] {
                                    InlineKeyboardButton.WithCallbackData(
                                        "Show subs count",
                                        $"{BotCommands.SubscribersCount} {user.Name}")
                                }
                        }),
                    ThumbUrl = user.Logo,
                    Description = $"Followers: {user.Followers}"
                };
                results.Add(article);
            }

            await telegramBotClient.AnswerInlineQueryAsync(
                request.InlineQueryId,
                results,
                cacheTime: 0,
                cancellationToken: cancellationToken);
        }
    }
}
