using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using TwitchSoft.Shared.ElasticSearch.Interfaces;
using TwitchSoft.Shared.Services.Repository.Interfaces;
using TwitchSoft.TelegramBot.MediatR.Models;

namespace TwitchSoft.TelegramBot.MediatR.Handlers
{
    public class InlineUsersSearchHandler : AsyncRequestHandler<InlineUsersSearch>
    {
        private readonly IUsersRepository usersRepository;
        private readonly ITelegramBotClient telegramBotClient;
        private readonly IESService eSService;

        public InlineUsersSearchHandler(
            IUsersRepository usersRepository,
            ITelegramBotClient telegramBotClient, 
            IESService eSService)
        {
            this.usersRepository = usersRepository;
            this.telegramBotClient = telegramBotClient;
            this.eSService = eSService;
        }

        protected override async Task Handle(InlineUsersSearch request, CancellationToken cancellationToken)
        {
            var searchUserText = request.SearchUserText.ToLower();
            var usersDB = usersRepository.SearchUsers(searchUserText);
            var usersES = eSService.SearchUsers(searchUserText);

            var uniqUsers = (await usersDB).Union(await usersES).Distinct();

            var results = new List<InlineQueryResultArticle>();
            foreach (var user in uniqUsers)
            {
                var article = new InlineQueryResultArticle(
                    id: user.Id.ToString(),
                    title: user.UserName,
                    inputMessageContent: new InputTextMessageContent(user.UserName))
                {
                    ReplyMarkup = new InlineKeyboardMarkup(new[]
                        {
                                new[] {
                                    InlineKeyboardButton.WithCallbackData(
                                        "Show messages",
                                        $"{BotCommands.UserMessages} {user.UserName}")
                                },
                                new[] {
                                    InlineKeyboardButton.WithCallbackData(
                                        "Show subs count",
                                        $"{BotCommands.SubscribersCount} {user.UserName}")
                                }
                        })
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
