using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using TwitchSoft.Shared.Services.Repository.Interfaces;
using TwitchSoft.TelegramBot.MediatR.Models;

namespace TwitchSoft.TelegramBot.MediatR.Handlers
{
    public class InlineUsersSearchHandler : AsyncRequestHandler<InlineUsersSearch>
    {
        private readonly IUsersRepository usersRepository;
        private readonly ITelegramBotClient telegramBotClient;

        public InlineUsersSearchHandler(
            IUsersRepository usersRepository,
            ITelegramBotClient telegramBotClient)
        {
            this.usersRepository = usersRepository;
            this.telegramBotClient = telegramBotClient;
        }

        protected override async Task Handle(InlineUsersSearch request, CancellationToken cancellationToken)
        {
            var users = await usersRepository.SearchUsers(request.SearchUserText);

            var results = new List<InlineQueryResultArticle>();
            foreach (var (Id, Username) in users)
            {
                var article = new InlineQueryResultArticle(
                    id: Id.ToString(),
                    title: Username,
                    inputMessageContent: new InputTextMessageContent(Username))
                {
                    ReplyMarkup = new InlineKeyboardMarkup(new[]
                        {
                                new[] {
                                    InlineKeyboardButton.WithCallbackData(
                                        "Show messages",
                                        $"{BotCommands.UserMessages} {Username}")
                                },
                                new[] {
                                    InlineKeyboardButton.WithCallbackData(
                                        "Show subs count",
                                        $"{BotCommands.Subscribers} {Username}")
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
