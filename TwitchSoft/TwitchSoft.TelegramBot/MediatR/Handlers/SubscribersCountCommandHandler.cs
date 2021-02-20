using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TwitchSoft.Shared.Services.Helpers;
using TwitchSoft.Shared.Services.Repository.Interfaces;
using TwitchSoft.TelegramBot.MediatR.Models;

namespace TwitchSoft.TelegramBot.MediatR.Handlers
{
    public class SubscribersCountCommandHandler : AsyncRequestHandler<SubscribersCountCommand>
    {
        private readonly ITelegramBotClient telegramBotClient;
        private readonly ISubscriptionsRepository subscriptionsRepository;
        public SubscribersCountCommandHandler(ITelegramBotClient telegramBotClient, ISubscriptionsRepository subscriptionsRepository)
        {
            this.telegramBotClient = telegramBotClient;
            this.subscriptionsRepository = subscriptionsRepository;
        }

        protected override async Task Handle(SubscribersCountCommand request, CancellationToken cancellationToken)
        {
            var dateFormat = "MMMM dd";
            var from = DateTime.UtcNow.AddMonths(-1).ConvertToMyTimezone().ToString(dateFormat);
            var to = DateTime.UtcNow.ConvertToMyTimezone().ToString(dateFormat);

            var subsCount = await subscriptionsRepository.GetMonthlySubscribersCountFor(request.ChannelName);

            var messageHeader = $"Subscriptions count {from} - {to}";

            await telegramBotClient.SendTextMessageAsync(
                        chatId: request.ChatId,
                        text: $"{messageHeader}\r\n{subsCount} subs on <b>{request.ChannelName}</b>",
                        parseMode: ParseMode.Html,
                        cancellationToken: cancellationToken
                    );
        }
    }
}
