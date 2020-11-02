﻿using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using TwitchSoft.TelegramBot.MediatR.Models;

namespace TwitchSoft.TelegramBot.MediatR.Handlers
{
    public class RequestAdditionalInfoCommandHandler : AsyncRequestHandler<RequestAdditionalInfoCommand>
    {
        private readonly ITelegramBotClient telegramBotClient;

        public RequestAdditionalInfoCommandHandler(ITelegramBotClient telegramBotClient)
        {
            this.telegramBotClient = telegramBotClient;
        }

        protected override async Task Handle(RequestAdditionalInfoCommand request, CancellationToken cancellationToken)
        {
            await telegramBotClient.SendTextMessageAsync(
                chatId: request.ChatId,
                text: $"Enter parameter [{request.ParamName}]",
                cancellationToken: cancellationToken
            );
        }
    }
}