using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using TwitchSoft.Shared.Services.Extensions;
using TwitchSoft.Shared.Services.Repository.Interfaces;
using TwitchSoft.Shared.Services.TwitchApi;
using User = TwitchSoft.Shared.Database.Models.User;
using static TwitchBotGrpc;
using System;
using System.Linq;
using TwitchSoft.Shared.Services.Helpers;
using StackExchange.Redis;
using TwitchSoft.Shared.ElasticSearch.Interfaces;

namespace TwitchSoft.TelegramBot
{
    public class TelegramBotCommandProcessor
    {
        private readonly TelegramBotClient telegramBotClient;
        private readonly IServiceScopeFactory scopeFactory;

        public const int TELEGRAM_MESSAGE_LIMIT = 4096;

        public TelegramBotCommandProcessor(
            TelegramBotClient telegramBotClient, 
            IServiceScopeFactory scopeFactory)
        {
            this.telegramBotClient = telegramBotClient;
            this.scopeFactory = scopeFactory;
        }

        public async Task ProcessUnknownCommand(string chatId)
        {
            string usage = $@"
Usage:
{BotCommands.UserMessages} [username] - покажет сообщения для пользователя
{BotCommands.AddChannel} [channel] - добавляет канал для отслеживания
{BotCommands.Subscribers} - выводит топ 100 каналов по кол-ву сабов
{BotCommands.SearchText} [text] - поиск по тексту сообщений";

            await telegramBotClient.SendTextMessageAsync(
                chatId,
                usage);
        }

        public async Task ProcessUserMessagesDigest(ChatId chatId, string userName)
        {
            await scopeFactory.RunInScope(async (scope) =>
            {
                var repository = scope.ServiceProvider.GetRequiredService<IRepository>();
                var redisClient = scope.ServiceProvider.GetService<ConnectionMultiplexer>();
                var esClient = scope.ServiceProvider.GetService<IESService>();

                var db = redisClient.GetDatabase(2);

                var chatIdentifier = chatId.Identifier.ToString();
                var dateValue = db.StringGet(chatIdentifier);
                var lastDateTime = dateValue.HasValue ? DateTime.Parse(dateValue) : DateTime.UtcNow.Date;
                db.StringSet(chatIdentifier, DateTime.UtcNow.ToString());

                var userIds = await repository.GetUserIds(userName);

                var count = 50;
                var messages = await esClient.GetMessages(userIds.First().Value, lastDateTime, count);

                var replyMessages = messages.GenerateReplyMessages();
                for (var i = 0; i < replyMessages.Count; i++)
                {
                    var replyMessage = replyMessages[i];
                    await telegramBotClient.SendTextMessageAsync(
                        chatId,
                        replyMessage,
                        parseMode: ParseMode.Html,
                        disableWebPagePreview: true,
                        replyMarkup: i == replyMessages.Count - 1
                        ? Utils.GenerateNavigationMarkup(BotCommands.UserMessages, userName, count, 0, messages.Count)
                        : null
                    );
                }
            });
        }

        public async Task ProcessUserMessagesCommand(ChatId chatId, string userName, string skipString = null)
        {
            await scopeFactory.RunInScope(async (scope) =>
            {
                var repository = scope.ServiceProvider.GetRequiredService<IRepository>();
                var esClient = scope.ServiceProvider.GetService<IESService>();
                var userIds = await repository.GetUserIds(userName);
                if (!userIds.Any())
                {
                    await telegramBotClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"User '{userName}' not found.",
                        parseMode: ParseMode.Html
                    );
                    return;
                }

                var count = 50;
                var skip = 0;
                if (!string.IsNullOrWhiteSpace(skipString))
                {
                    skip = int.Parse(skipString);
                };
                var messages = await esClient.GetMessages(userIds.First().Value, skip, count);

                var replyMessages = messages.GenerateReplyMessages();
                if (!replyMessages.Any())
                {
                    await telegramBotClient.SendTextMessageAsync(
                        chatId,
                        "No messages were found."
                    );
                }
                for (var i = 0; i < replyMessages.Count; i++)
                {
                    var replyMessage = replyMessages[i];
                    await telegramBotClient.SendTextMessageAsync(
                        chatId,
                        replyMessage,
                        parseMode: ParseMode.Html,
                        disableWebPagePreview: true,
                        replyMarkup: i == replyMessages.Count - 1
                        ? Utils.GenerateNavigationMarkup(BotCommands.UserMessages, userName, count, skip, messages.Count)
                        : null
                    );
                }
            });
        }

        public async Task ProcessInlineQueryCommand(string inlineQueryId, string queryText)
        {
            List<User> users = null;
            await scopeFactory.RunInScope(async (scope) =>
            {
                var repository = scope.ServiceProvider.GetRequiredService<IRepository>();

                users = await repository.SearchUsers(queryText);
            });

            var results = new List<InlineQueryResultArticle>();
            foreach (var user in users)
            {
                var article = new InlineQueryResultArticle(
                    id: user.Id.ToString(),
                    title: user.Username,
                    inputMessageContent: new InputTextMessageContent(user.Username))
                {
                    ReplyMarkup = new InlineKeyboardMarkup(new[]
                        {
                                new[] {
                                    InlineKeyboardButton.WithCallbackData(
                                        "Show messages",
                                        $"{BotCommands.UserMessages} {user.Username}")
                                },
                                new[] {
                                    InlineKeyboardButton.WithCallbackData(
                                        "Show subs count",
                                        $"{BotCommands.Subscribers} {user.Username}")
                                }
                        })
                };
                results.Add(article);
            }

            await telegramBotClient.AnswerInlineQueryAsync(
                inlineQueryId,
                results,
                //isPersonal: true,
                cacheTime: 0);
        }

        public async Task ProcessNewChannelCommand(ChatId chatId, string channelName)
        {
            await scopeFactory.RunInScope(async (scope) =>
            {

                var twitchApiService = scope.ServiceProvider.GetRequiredService<ITwitchApiService>();
                var channel = await twitchApiService.GetChannelByName(channelName);
                if (channel == null)
                {
                    await telegramBotClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: $"Channel '{channelName}' was not found.",
                            parseMode: ParseMode.Html
                        );
                    return;
                }
                var repository = scope.ServiceProvider.GetRequiredService<IRepository>();
                var alreadyExist = await repository.AddChannelToTrack(channel);
                if (alreadyExist)
                {
                    await telegramBotClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: $"Channel '{channelName}' has already been tracking.",
                            parseMode: ParseMode.Html
                        );
                }
                else
                {

                    Channel grpcChannel = new Channel("twitchbot", 80, ChannelCredentials.Insecure);
                    var client = new TwitchBotGrpcClient(grpcChannel);
                    await client.JoinChannelAsync(new JoinChannelRequest { Channelname = channelName });

                    await telegramBotClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: $"Channel '{channelName}' successfully added.",
                            parseMode: ParseMode.Html
                        );
                }
            });
        }

        public async Task ProcessSearchTextCommand(ChatId chatId, string searchText, string skipString = null)
        {
            await scopeFactory.RunInScope(async (scope) =>
            {
                var repository = scope.ServiceProvider.GetRequiredService<IRepository>();
                var esClient = scope.ServiceProvider.GetService<IESService>();

                var count = 50;
                var skip = 0;
                if (!string.IsNullOrWhiteSpace(skipString))
                {
                    skip = int.Parse(skipString);
                };
                var messages = await esClient.SearchMessages(searchText, skip, count);

                var replyMessages = messages.GenerateReplyMessages();
                if (!replyMessages.Any())
                {
                    await telegramBotClient.SendTextMessageAsync(
                        chatId,
                        "No messages were found."
                    );
                }
                for (var i = 0; i < replyMessages.Count; i++)
                {
                    var replyMessage = replyMessages[i];
                    await telegramBotClient.SendTextMessageAsync(
                        chatId,
                        replyMessage,
                        parseMode: ParseMode.Html,
                        disableWebPagePreview: true,
                        replyMarkup: i == replyMessages.Count - 1
                        ? Utils.GenerateNavigationMarkup(BotCommands.SearchText, searchText, count, skip, messages.Count)
                        : null
                    );
                }
            });
        }

        public async Task ProcessSubscribersCommand(ChatId chatId, string paramString = null)
        {
            await scopeFactory.RunInScope(async (scope) =>
            {
                var repository = scope.ServiceProvider.GetRequiredService<IRepository>();
                var count = 100;
                var skip = 0;
                string channel = null;
                if (!string.IsNullOrWhiteSpace(paramString))
                {
                    if (!int.TryParse(paramString, out skip))
                    {
                        channel = paramString;
                    }
                };

                var dateFormat = "MMMM dd";
                var from = DateTime.UtcNow.AddMonths(-1).ConvertToMyTimezone().ToString(dateFormat);
                var to = DateTime.UtcNow.ConvertToMyTimezone().ToString(dateFormat);

                if (string.IsNullOrWhiteSpace(channel))
                {
                    var channelSubs = await repository.GetTopChannelsBySubscribers(skip, count);

                    var messageHeader = $"Top Month Subscriptions count {from} - {to}";
                    var messageBody = string.Join("\r\n", channelSubs.Select(_ => $"{_.SubsCount} subs on <b>{_.Channel}</b>"));

                    await telegramBotClient.SendTextMessageAsync(
                                chatId: chatId,
                                text: $"{messageHeader}\r\n{messageBody}",
                                parseMode: ParseMode.Html,
                                replyMarkup: Utils.GenerateNavigationMarkup(BotCommands.Subscribers, string.Empty, count, skip, channelSubs.Count)
                            );
                }
                else
                {
                    var subsCount = await repository.GetSubscribersCountFor(channel);

                    var messageHeader = $"Subscriptions count {from} - {to}";

                    await telegramBotClient.SendTextMessageAsync(
                                chatId: chatId,
                                text: $"{messageHeader}\r\n{subsCount} subs on <b>{ channel}</b>",
                                parseMode: ParseMode.Html
                            );
                }
            });
        }
    }
}
