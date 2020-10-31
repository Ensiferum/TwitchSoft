using MediatR;
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
    public class TopSubscribersCommandHandler : AsyncRequestHandler<TopSubscribersCommand>
    {
        private readonly ITelegramBotClient telegramBotClient;
        private readonly ISubscriptionsRepository subscriptionsRepository;
        public TopSubscribersCommandHandler(ITelegramBotClient telegramBotClient, ISubscriptionsRepository subscriptionsRepository)
        {
            this.telegramBotClient = telegramBotClient;
            this.subscriptionsRepository = subscriptionsRepository;
        }

        protected override async Task Handle(TopSubscribersCommand request, CancellationToken cancellationToken)
        {
            var chatId = request.ChatId;
            var paramString = request.ParamString;

            var count = 10;
            var skip = 0;
            string channel = null;
            if (!string.IsNullOrWhiteSpace(paramString))
            {
                if (!int.TryParse(paramString, out skip))
                {
                    channel = paramString;
                }
            };

            var dateFormat = "MMMM dd";
            var from = DateTime.UtcNow.AddMonths(-1).ConvertToMyTimezone().ToString(dateFormat);
            var to = DateTime.UtcNow.ConvertToMyTimezone().ToString(dateFormat);

            if (string.IsNullOrWhiteSpace(channel))
            {
                var channelSubs = await subscriptionsRepository.GetTopChannelsBySubscribers(skip, count);

                var messageHeader = $"Top Month Subscriptions count {from} - {to}";
                var messageBody = string.Join("\r\n", channelSubs.Select(_ => $"{_.SubsCount} subs on <b>{_.Channel}</b>"));

                await telegramBotClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: $"{messageHeader}\r\n{messageBody}",
                            parseMode: ParseMode.Html,
                            replyMarkup: Utils.GenerateNavigationMarkup(BotCommands.Subscribers, string.Empty, count, skip, channelSubs.Count()),
                            cancellationToken: cancellationToken
                        );
            }
            else
            {
                var subsCount = await subscriptionsRepository.GetSubscribersCountFor(channel);

                var messageHeader = $"Subscriptions count {from} - {to}";

                await telegramBotClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: $"{messageHeader}\r\n{subsCount} subs on <b>{channel}</b>",
                            parseMode: ParseMode.Html,
                            cancellationToken: cancellationToken
                        );
            }
        }
    }
}
