using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TwitchSoft.TelegramBot
{
    public class TelegramBot
    {
        private readonly ILogger<TelegramBot> logger;
        private readonly ITelegramBotClient telegramBotClient;
        private readonly MessageProcessor messageProcessor;
        private CancellationTokenSource _cts;

        public TelegramBot(
            ILogger<TelegramBot> logger,
            ITelegramBotClient telegramBotClient,
            MessageProcessor messageProcessor)
        {
            this.logger = logger;
            this.telegramBotClient = telegramBotClient;
            this.messageProcessor = messageProcessor;
        }

        public void Start()
        {
            _cts?.Cancel();
            Connect();
        }

        public void Stop()
        {
            _cts?.Cancel();
        }

        private void Connect()
        {
            _cts = new CancellationTokenSource();
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
                
            };

            telegramBotClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                _cts.Token);
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message || update.Type == UpdateType.EditedMessage)
            {
                await messageProcessor.ProcessMessage(update.Message);
            }
            else if (update.Type == UpdateType.CallbackQuery)
            {
                await messageProcessor.ProcessCallbackQuery(update.CallbackQuery);
            }
            else if (update.Type == UpdateType.InlineQuery)
            {
                await messageProcessor.ProcessInlineQuery(update.InlineQuery);
            }
        }

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogError(exception, "TelegramBot error occured");
            return Task.CompletedTask;
        }
    }
}