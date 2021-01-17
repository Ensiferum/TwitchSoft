﻿using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TwitchSoft.Shared.Services.Helpers;
using TwitchSoft.Shared.Services.Repository.Interfaces;
using TwitchSoft.TelegramBot.MediatR.Models;

namespace TwitchSoft.TelegramBot.MediatR.Handlers
{
    public class DailyNewSubscribersCountCommandHandler : AsyncRequestHandler<DailyNewSubscribersCountCommand>
    {
        private readonly ITelegramBotClient telegramBotClient;
        private readonly ISubscriptionsRepository subscriptionsRepository;
        public DailyNewSubscribersCountCommandHandler(ITelegramBotClient telegramBotClient, ISubscriptionsRepository subscriptionsRepository)
        {
            this.telegramBotClient = telegramBotClient;
            this.subscriptionsRepository = subscriptionsRepository;
        }

        protected override async Task Handle(DailyNewSubscribersCountCommand request, CancellationToken cancellationToken)
        {
            var chatId = request.ChatId;
            var skip = request.Skip;
            var count = 10;

            var dateFormat = "MMMM dd";
            var from = DateTime.UtcNow.AddDays(-1);
            var to = DateTime.UtcNow;

            var channelSubs = await subscriptionsRepository.GetSubscribersCount(skip, count, from);

            var fromString = from.ConvertToMyTimezone().ToString(dateFormat);
            var toString = to.ConvertToMyTimezone().ToString(dateFormat);

            var messageHeader = $"Daily New Subscriptions count {fromString} - {toString}";
            var messageBody = string.Join("\r\n", channelSubs.Select(_ => $"{_.SubsCount} subs on <b>{_.Channel}</b>"));

            await telegramBotClient.SendTextMessageAsync(
                chatId: chatId,
                text: $"{messageHeader}\r\n{messageBody}",
                parseMode: ParseMode.Html,
                replyMarkup: InlineUtils.GenerateNavigationMarkup(BotCommands.DailyNewSubs, string.Empty, count, skip, channelSubs.Count()),
                cancellationToken: cancellationToken
            );
        }
    }
}
