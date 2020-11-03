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
using TwitchSoft.TelegramBot.TgCommands;

namespace TwitchSoft.TelegramBot
{
    public class MessageProcessor
    {
        private readonly ILogger<MessageProcessor> logger;
        private readonly ITelegramBotClient telegramBotClient;
        private readonly IMediator mediator;
        private readonly IMapper mapper;
        private readonly IEnumerable<BaseTgCommand> tgCommands;

        private readonly ConcurrentDictionary<string, BotCommand> usersPrevCommands = new ConcurrentDictionary<string, BotCommand>();

        public MessageProcessor(
            ILogger<MessageProcessor> logger,
            ITelegramBotClient telegramBotClient,
            IMediator mediator,
            IMapper mapper, 
            IEnumerable<BaseTgCommand> tgCommands)
        {
            this.logger = logger;
            this.telegramBotClient = telegramBotClient;
            this.mediator = mediator;
            this.mapper = mapper;
            this.tgCommands = tgCommands;
        }

        public async Task ProcessMessage(Message message)
        {
            try
            {
                var chatId = message.Chat.Id.ToString();
                var messageText = message?.Text ?? string.Empty;

                var messageSplitted = (message?.Text ?? string.Empty).Split(" ", StringSplitOptions.RemoveEmptyEntries);
                var command = messageSplitted.FirstOrDefault();
                var parameters = messageSplitted.Length > 1 ?
                    new[] { string.Join(" ", messageSplitted.Skip(1)) } :
                    Enumerable.Empty<string>();

                logger.LogInformation($"Received: {message.Text} from: {message.Chat.Username}.");

                if (messageText.StartsWith("/") == false && usersPrevCommands.TryGetValue(chatId, out BotCommand botCommand))
                {
                    command = $"/{botCommand}";
                    parameters = new[] { messageText };
                }

                _ = usersPrevCommands.TryRemove(chatId, out _);

                await ProcessQuery(new ChatInfo
                {
                    ChatId = chatId,
                    Username = message.Chat.Username
                }, command, parameters.ToArray());
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

                var messageSplitted = message.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                var chatId = callbackQuery.Message != null ? callbackQuery.Message.Chat.Id : callbackQuery.From.Id;

                var command = messageSplitted.FirstOrDefault();
                var parameters = messageSplitted.Length > 1 ? 
                    messageSplitted.Skip(1).FirstOrDefault().Split(InlineUtils.InlineParamSeparator, StringSplitOptions.RemoveEmptyEntries) :
                    Enumerable.Empty<string>();

                await ProcessQuery(new ChatInfo
                {
                    ChatId = chatId.ToString(),
                    Username = callbackQuery.From.Username
                }, command, parameters.ToArray());

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
            if (command.StartsWith("/") && Enum.TryParse(command.TrimStart('/'), true, out BotCommand botCommand))
            {
                var tgCommand = tgCommands.FirstOrDefault(_ => _.BotCommand == botCommand);
                if (tgCommand == null)
                {
                    logger.LogWarning($"{botCommand} is not handled. Add handler for this command");
                    return;
                }

                if (tgCommand is ParameterizedTgCommand paramTgCommand && paramTgCommand.IsValidParameters(parameters) == false)
                {
                    usersPrevCommands[chatInfo.ChatId] = botCommand;
                    await paramTgCommand.RequestAdditionalParameters(chatInfo.ChatId);
                    return;
                }

                await tgCommand.Execute(chatInfo.ChatId, parameters);
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

        private record ChatInfo
        {
            public string ChatId { get; init; }
            public string Username { get; init; }
        }
    }
}
