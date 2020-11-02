using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TwitchSoft.TelegramBot.MediatR.Models;

namespace TwitchSoft.TelegramBot
{
    public class MessageProcessor
    {
        private readonly ILogger<MessageProcessor> logger;
        private readonly ITelegramBotClient telegramBotClient;
        private readonly IMediator mediator;
        private readonly IMapper mapper;

        public static readonly Dictionary<BotCommand, string> CommandsWithRequiredParameters = new Dictionary<BotCommand, string>
            {
                { BotCommand.UserMessages, "username" },
                { BotCommand.AddChannel, "channel" },
                { BotCommand.SubscribersCount, "channel" },
                { BotCommand.SearchText, "text" },
            };

        private readonly ConcurrentDictionary<string, BotCommand> usersPrevCommands = new ConcurrentDictionary<string, BotCommand>();

        public MessageProcessor(
            ILogger<MessageProcessor> logger,
            ITelegramBotClient telegramBotClient,
            IMediator mediator, 
            IMapper mapper)
        {
            this.logger = logger;
            this.telegramBotClient = telegramBotClient;
            this.mediator = mediator;
            this.mapper = mapper;
        }

        public async Task ProcessMessage(Message message)
        {
            try
            {
                var chatId = message.Chat.Id.ToString();
                var messageText = message?.Text ?? string.Empty;

                var messageSplitted = (message?.Text ?? string.Empty).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var command = messageSplitted.FirstOrDefault();
                var parameters = messageSplitted.Skip(1).ToArray();

                logger.LogInformation($"Received: {message.Text} from: {message.Chat.Username}.");

                if (messageText.StartsWith("/") == false && usersPrevCommands.TryGetValue(chatId, out BotCommand botCommand))
                {
                    command = $"/{botCommand}";
                    parameters = messageSplitted.ToArray();
                }

                _ = usersPrevCommands.TryRemove(chatId, out _);

                await ProcessQuery(new ChatInfo
                {
                    ChatId = chatId,
                    Username = message.Chat.Username
                }, command, parameters);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ProcessMessage");
            }
        }

        public async Task ProcessInlineQuery(InlineQuery inlineQuery)
        {
            try
            {
                logger.LogInformation($"ProcessInlineQuery: {inlineQuery.Query}");

                var ius = mapper.Map<InlineUsersSearch>(inlineQuery);
                await mediator.Send(ius);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ProcessInlineQuery");
            }
        }

        public async Task ProcessCallbackQuery(CallbackQuery callbackQuery)
        {
            try
            {
                var message = callbackQuery.Data;

                logger.LogInformation($"Received callback query: {message} from: {callbackQuery.From.Username}.");

                var messageSplitted = message.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var chatId = callbackQuery.Message != null ? callbackQuery.Message.Chat.Id : callbackQuery.From.Id;

                var command = messageSplitted.FirstOrDefault();
                var parameters = messageSplitted.Skip(1).ToArray();

                await ProcessQuery(new ChatInfo
                {
                    ChatId = chatId.ToString(),
                    Username = callbackQuery.From.Username
                }, command, parameters);

                try
                {
                    await telegramBotClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"{callbackQuery.Id} callback query expired");
                }
                logger.LogInformation($"ProcessCallbackQuery: {callbackQuery.From.Username} Data: {callbackQuery.Data}");

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ProcessCallbackQuery");
            }
        }

        private async Task ProcessQuery(ChatInfo chatInfo, string command, string[] parameters)
        {
            if (Enum.TryParse(command.TrimStart('/'), true, out BotCommand botCommand))
            {
                if (CommandsWithRequiredParameters.ContainsKey(botCommand) && parameters.Length == 0)
                {
                    await RequestAdditionalData(chatInfo.ChatId, botCommand);
                    return;
                }

                switch (botCommand)
                {
                    case BotCommand.UserMessages:
                        await GetUserMessages(chatInfo.ChatId, parameters.ElementAtOrDefault(0), parameters.ElementAtOrDefault(1));
                        break;
                    case BotCommand.SubscribersCount:
                        await GetSubscribersCount(chatInfo.ChatId, parameters.ElementAtOrDefault(0));
                        break;
                    case BotCommand.TopBySubscribers:
                        await ListTopBySubscribers(chatInfo.ChatId, parameters.ElementAtOrDefault(0));
                        break;
                    case BotCommand.SearchText:
                        await SearchText(chatInfo.ChatId, parameters.ElementAtOrDefault(0), parameters.ElementAtOrDefault(1));
                        break;
                    case BotCommand.AddChannel:
                        await AddNewChannel(chatInfo.ChatId, parameters.ElementAtOrDefault(0));
                        break;
                }
            }
            else
            {
                await SendUnknownCommand(chatInfo.ChatId);
                logger.LogWarning($"Received unknown command from {chatInfo.Username}:\r\n{string.Join(" ", parameters.Prepend(command))}");
            }
        }

        public async Task SendUnknownCommand(string chatId)
        {
            await mediator.Send(new UnknownCommand
            {
                ChatId = chatId,
            });
        }

        public async Task GetUserMessages(string chatId, string userName, string skipString = null)
        {
            await mediator.Send(new UserMessagesCommand
            {
                ChatId = chatId,
                Username = userName,
                SkipString = skipString
            });
        }

        public async Task AddNewChannel(string chatId, string channelName)
        {
            await mediator.Send(new NewChannelCommand
            {
                ChatId = chatId,
                ChannelName = channelName
            });
        }

        public async Task SearchText(string chatId, string searchText, string skipString = null)
        {
            await mediator.Send(new SearchTextCommand
            {
                ChatId = chatId,
                SearchText = searchText,
                SkipString = skipString
            });
        }

        public async Task ListTopBySubscribers(string chatId, string skipString = null)
        {
            await mediator.Send(new TopBySubscribersCommand
            {
                ChatId = chatId,
                SkipString = skipString,
            });
        }

        public async Task GetSubscribersCount(string chatId, string channelName)
        {
            await mediator.Send(new SubscribersCountCommand
            {
                ChatId = chatId,
                ChannelName = channelName,
            });
        }

        public async Task RequestAdditionalData(string chatId, BotCommand prevBotCommand)
        {
            usersPrevCommands[chatId] = prevBotCommand;
            await mediator.Send(new RequestAdditionalInfoCommand
            {
                ChatId = chatId,
                ParamName = CommandsWithRequiredParameters[prevBotCommand]
            });
        }

        private record ChatInfo
        {
            public string ChatId { get; init; }
            public string Username { get; init; }
        }
    }
}
