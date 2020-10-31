﻿using AutoMapper;
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

                logger.LogInformation($"Received: {message.Text} from: {message.Chat.Username}.");

                if (await TryHandleUserState(chatId, message)) return;

                var messageSplitted = (message?.Text ?? string.Empty).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                switch (messageSplitted.FirstOrDefault())
                {
                    case BotCommands.UserMessages:
                        var userName = messageSplitted.ElementAtOrDefault(1);
                        if (string.IsNullOrEmpty(userName))
                        {
                            usersState[chatId] = BotState.WaitingForUserName;
                            await telegramBotClient.SendTextMessageAsync(
                                chatId: chatId,
                                text: "Enter username please"
                            );
                            return;
                        }
                        await mediator.Send(new UserMessagesCommand
                        {
                            ChatId = chatId,
                            Username = userName,
                        });
                        break;
                    case BotCommands.AddChannel:
                        var channelName = messageSplitted.ElementAtOrDefault(1);
                        if (string.IsNullOrEmpty(channelName))
                        {
                            usersState[chatId] = BotState.WaitingForNewChannel;
                            await telegramBotClient.SendTextMessageAsync(
                                chatId: chatId,
                                text: "Enter channel please"
                            );
                            return;
                        }
                        await mediator.Send(new NewChannelCommand
                        {
                            ChatId = chatId,
                            ChannelName = channelName,
                        });
                        break;
                    case BotCommands.Subscribers:
                        await mediator.Send(new TopSubscribersCommand
                        {
                            ChatId = chatId,
                        });
                        break;
                    case BotCommands.SearchText:
                        var searchText = messageSplitted.ElementAtOrDefault(1);
                        if (string.IsNullOrEmpty(searchText))
                        {
                            usersState[chatId] = BotState.WaitingForMessage;
                            await telegramBotClient.SendTextMessageAsync(
                                chatId: chatId,
                                text: "Enter search text please"
                            );
                            return;
                        }
                        await mediator.Send(new SearchTextCommand
                        {
                            ChatId = chatId,
                            SearchText = searchText,
                        });
                        break;
                    default:
                        await mediator.Send(new UnknownCommand
                        {
                            ChatId = chatId,
                        });

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
                        await mediator.Send(new UserMessagesCommand
                        {
                            ChatId = chatId.ToString(),
                            Username = messageSplitted.ElementAtOrDefault(1),
                            SkipString = messageSplitted.ElementAtOrDefault(2)
                        });
                        break;
                    case BotCommands.Subscribers:
                        await mediator.Send(new TopSubscribersCommand
                        {
                            ChatId = chatId.ToString(),
                            ParamString = messageSplitted.ElementAtOrDefault(1),
                        });
                        break;
                    case BotCommands.SearchText:
                        await mediator.Send(new SearchTextCommand
                        {
                            ChatId = chatId.ToString(),
                            SearchText = messageSplitted.ElementAtOrDefault(1),
                            SkipString = messageSplitted.ElementAtOrDefault(2)
                        });
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
                        var userName = message.Text;
                        await mediator.Send(new UserMessagesCommand
                        {
                            ChatId = chatId,
                            Username = userName
                        });
                        break;
                    case BotState.WaitingForNewChannel:
                        var channelName = message.Text;
                        await mediator.Send(new NewChannelCommand
                        {
                            ChatId = chatId,
                            ChannelName = channelName
                        });
                        break;
                    case BotState.WaitingForMessage:
                        var searchText = message.Text;
                        await mediator.Send(new SearchTextCommand
                        {
                            ChatId = chatId,
                            SearchText = searchText
                        });
                        break;
                }
                _ = usersState.TryRemove(chatId, out _);
                return true;
            }
            return false;
        }
    }
}