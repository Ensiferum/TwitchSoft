using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using TwitchSoft.TelegramBot.MediatR.Models;

namespace TwitchSoft.TelegramBot
{
    public class TelegramBot
    {
        private readonly ILogger<TelegramBot> logger;
        private readonly ITelegramBotClient telegramBotClient;
        private readonly MessageProcessor messageProcessor;

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
            Connect();
        }

        public void Stop()
        {
            Disconnect();
        }
        private void Connect()
        {
            InitTelegramBotEvents();
            telegramBotClient.StartReceiving();
        }

        private void Disconnect()
        {
            telegramBotClient.StopReceiving();
        }
        
        private void InitTelegramBotEvents()
        {
            telegramBotClient.OnMessage += Bot_OnMessage;
            telegramBotClient.OnMessageEdited += Bot_OnMessage;
            telegramBotClient.OnCallbackQuery += Bot_OnCallbackQuery;
            telegramBotClient.OnInlineQuery += Bot_OnInlineQuery;
            telegramBotClient.OnReceiveError += Bot_OnReceiveError;
        }

        private void Bot_OnReceiveError(object sender, ReceiveErrorEventArgs e)
        {
            logger.LogError(e.ApiRequestException, $"Bot_OnReceiveError");
        }

        private async void Bot_OnInlineQuery(object sender, InlineQueryEventArgs e)
        {
            await messageProcessor.ProcessInlineQuery(e.InlineQuery);
        }

        private async void Bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            await messageProcessor.ProcessCallbackQuery(e.CallbackQuery);
        }

        private async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            await messageProcessor.ProcessMessage(e.Message);
        }
    }
}