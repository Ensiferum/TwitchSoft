using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
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

        private readonly ConcurrentDictionary<string, BotState> usersState = new ConcurrentDictionary<string, BotState>();

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
                string channelName;

                logger.LogInformation($"Received: {message.Text} from: {message.Chat.Username}.");

                if (await TryHandleUserState(chatId, message)) return;

                var messageSplitted = (message?.Text ?? string.Empty).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                switch (messageSplitted.FirstOrDefault())
                {
                    case BotCommands.UserMessages:
                        var userName = messageSplitted.ElementAtOrDefault(1);
                        if (string.IsNullOrEmpty(userName))
                        {
                            await RequestAdditionalData(chatId, "Enter username please", BotState.WaitingForUserName);
                            return;
                        }
                        await GetUserMessages(chatId, userName);
                        break;
                    case BotCommands.AddChannel:
                        channelName = messageSplitted.ElementAtOrDefault(1);
                        if (string.IsNullOrEmpty(channelName))
                        {
                            await RequestAdditionalData(chatId, "Enter channel please", BotState.WaitingForNewChannel);
                            return;
                        }
                        await AddNewChannel(chatId, channelName);
                        break;
                    case BotCommands.TopBySubscribers:
                        await ListTopBySubscribers(chatId);
                        break;
                    case BotCommands.SubscribersCount:
                        channelName = messageSplitted.ElementAtOrDefault(1);
                        if (string.IsNullOrEmpty(channelName))
                        {
                            await RequestAdditionalData(chatId, "Enter channel please", BotState.WaitingForSubscribersCountChannelName);
                            return;
                        }
                        await GetSubscribersCount(chatId, channelName);
                        break;
                    case BotCommands.SearchText:
                        var searchText = messageSplitted.ElementAtOrDefault(1);
                        if (string.IsNullOrEmpty(searchText))
                        {
                            await RequestAdditionalData(chatId, "Enter search text please", BotState.WaitingForMessage);
                            return;
                        }
                        await SearchText(chatId, searchText);
                        break;
                    default:
                        await SendUnknownCommand(chatId);

                        logger.LogWarning($"Received unknown command: {message.Text}");
                        break;

                }
                //add following commands
                //online, etc

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
                switch (messageSplitted.First())
                {
                    case BotCommands.UserMessages:
                        await GetUserMessages(chatId.ToString(), messageSplitted.ElementAtOrDefault(1), messageSplitted.ElementAtOrDefault(2));
                        break;
                    case BotCommands.SubscribersCount:
                        await GetSubscribersCount(chatId.ToString(), messageSplitted.ElementAtOrDefault(1));
                        break;
                    case BotCommands.TopBySubscribers:
                        await ListTopBySubscribers(chatId.ToString(), messageSplitted.ElementAtOrDefault(1));
                        break;
                    case BotCommands.SearchText:
                        await SearchText(chatId.ToString(), messageSplitted.ElementAtOrDefault(1), messageSplitted.ElementAtOrDefault(2));
                        break;
                    default:
                        logger.LogWarning($"Received unknown command: {message} from: {callbackQuery.From.Username}.");
                        break;
                }

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

        private async Task<bool> TryHandleUserState(string chatId, Message message)
        {
            if (usersState.TryGetValue(chatId, out BotState userState) && message.Text?.StartsWith("/") == false)
            {
                switch (userState)
                {
                    case BotState.WaitingForUserName:
                        await GetUserMessages(chatId, message.Text);
                        break;
                    case BotState.WaitingForNewChannel:
                        await AddNewChannel(chatId, message.Text);
                        break;
                    case BotState.WaitingForMessage:
                        await SearchText(chatId, message.Text);
                        break;
                    case BotState.WaitingForSubscribersCountChannelName:
                        await GetSubscribersCount(chatId, message.Text);
                        break;
                }
                _ = usersState.TryRemove(chatId, out _);
                return true;
            }
            return false;
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

        public async Task RequestAdditionalData(string chatId, string messageText, BotState newBotState)
        {
            usersState[chatId] = newBotState;
            await telegramBotClient.SendTextMessageAsync(
                chatId: chatId,
                text: messageText
            );
        }
    }
}
