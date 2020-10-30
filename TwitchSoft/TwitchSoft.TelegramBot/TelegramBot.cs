using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using TwitchSoft.Shared.Services.Models.Telegram;

namespace TwitchSoft.TelegramBot
{
    public class TelegramBot
    {
        private readonly ILogger<TelegramBot> logger;
        private readonly IServiceScopeFactory scopeFactory;
        private readonly BotSettings BotSettings;

        private TelegramBotClient telegramBotClient;
        private TelegramBotCommandProcessor telegramBotCommandProcessor;
        private readonly ConcurrentDictionary<string, BotState> usersState = new ConcurrentDictionary<string, BotState>();

        public TelegramBot(
            ILogger<TelegramBot> logger, 
            IOptions<BotSettings> options, 
            IServiceScopeFactory scopeFactory)
        {
            this.logger = logger;
            this.scopeFactory = scopeFactory;
            BotSettings = options.Value;
        }

        public void Start()
        {
            Connect();
        }

        public void Stop()
        {
            Disconnect();
        }
        private void Connect()
        {
            if (telegramBotClient != null && telegramBotClient.IsReceiving == true)
            {
                telegramBotClient.StopReceiving();
            }

            InitTelegramBotClient();
            telegramBotClient.StartReceiving();
        }

        private void Disconnect()
        {
            telegramBotClient.StopReceiving();
        }
        
        private void InitTelegramBotClient()
        {
            telegramBotClient = new TelegramBotClient(BotSettings.BotOAuthToken);

            telegramBotCommandProcessor = new TelegramBotCommandProcessor(
                telegramBotClient, 
                scopeFactory);

            telegramBotClient.OnMessage += Bot_OnMessage;
            telegramBotClient.OnMessageEdited += Bot_OnMessage;
            telegramBotClient.OnCallbackQuery += Bot_OnCallbackQuery;
            telegramBotClient.OnInlineQuery += Bot_OnInlineQuery;
            telegramBotClient.OnReceiveError += Bot_OnReceiveError;
        }

        public Task SentDigest(DigestInfoRequest digestInfo)
        {
            return telegramBotCommandProcessor.ProcessUserMessagesDigest(digestInfo.ChatId, digestInfo.Username);
        }

        private void Bot_OnReceiveError(object sender, ReceiveErrorEventArgs e)
        {
            logger.LogError(e.ApiRequestException, $"Bot_OnReceiveError");
        }

        private async void Bot_OnInlineQuery(object sender, InlineQueryEventArgs e)
        {
            try
            {
                var queryText = e.InlineQuery.Query;
                var inlineQueryId = e.InlineQuery.Id;

                logger.LogInformation($"Bot_OnInlineQuery: {queryText}");

                await telegramBotCommandProcessor.ProcessInlineQueryCommand(inlineQueryId, queryText);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Bot_OnInlineQuery");
            }
        }

        private async void Bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            try
            {
                var message = e.CallbackQuery.Data;

                logger.LogInformation($"Received callback query: {message} from: {e.CallbackQuery.From.Username}.");

                await ProcessCallbackQuery(e, message);

                try
                {
                    await telegramBotClient.AnswerCallbackQueryAsync(e.CallbackQuery.Id);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"{e.CallbackQuery.Id} callback query expired");
                }
                logger.LogInformation($"Bot_OnCallbackQuery: {e.CallbackQuery.From.Username} Data: {e.CallbackQuery.Data}");

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Bot_OnCallbackQuery");
            }
        }

        private async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            try
            {
                var message = e.Message;
                var chatId = e.Message.Chat.Id.ToString();

                logger.LogInformation($"Received: {message.Text} from: {e.Message.Chat.Username}.");

                if (await TryHandleUserState(chatId, message)) return;

                await ProcessMessage(message, chatId);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Bot_OnMessage");
            }
            
        }

        private async Task ProcessCallbackQuery(CallbackQueryEventArgs e, string message)
        {
            var messageSplitted = message.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var chatId = e.CallbackQuery.Message != null ? e.CallbackQuery.Message.Chat.Id : e.CallbackQuery.From.Id;
            switch (messageSplitted.First())
            {
                case BotCommands.UserMessages:
                    await telegramBotCommandProcessor.ProcessUserMessagesCommand(chatId, messageSplitted.ElementAtOrDefault(1), messageSplitted.ElementAtOrDefault(2));
                    break;
                case BotCommands.Subscribers:
                    await telegramBotCommandProcessor.ProcessSubscribersCommand(chatId, messageSplitted.ElementAtOrDefault(1));
                    break;
                case BotCommands.SearchText:
                    await telegramBotCommandProcessor.ProcessSearchTextCommand(chatId, messageSplitted.ElementAtOrDefault(1), messageSplitted.ElementAtOrDefault(2));
                    break;
                default:
                    logger.LogWarning($"Received unknown command: {message} from: {e.CallbackQuery.From.Username}.");
                    break;

            }
        }

        private async Task ProcessMessage(Message message, string chatId)
        {
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
                    await telegramBotCommandProcessor.ProcessUserMessagesCommand(chatId, userName);
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
                    await telegramBotCommandProcessor.ProcessNewChannelCommand(chatId, channelName);
                    break;
                case BotCommands.Subscribers:
                    await telegramBotCommandProcessor.ProcessSubscribersCommand(chatId);
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
                    await telegramBotCommandProcessor.ProcessSearchTextCommand(chatId, searchText);
                    break;
                default:
                    await telegramBotCommandProcessor.ProcessUnknownCommand(chatId);

                    logger.LogWarning($"Received unknown command: {message.Text}");
                    break;

            }
            //add following commands
            //online, etc
        }

        private async Task<bool> TryHandleUserState(string chatId, Message message)
        {
            if (usersState.TryGetValue(chatId, out BotState userState) && message.Text?.StartsWith("/") == false)
            {
                switch (userState)
                {
                    case BotState.WaitingForUserName:
                        var userName = message.Text;
                        await telegramBotCommandProcessor.ProcessUserMessagesCommand(chatId, userName);
                        break;
                    case BotState.WaitingForNewChannel:
                        var channelName = message.Text;
                        await telegramBotCommandProcessor.ProcessNewChannelCommand(chatId, channelName);
                        break;
                    case BotState.WaitingForMessage:
                        var searchText = message.Text;
                        await telegramBotCommandProcessor.ProcessSearchTextCommand(chatId, searchText);
                        break;
                }
                _ = usersState.TryRemove(chatId, out _);
                return true;
            }
            return false;
        }
    }
}