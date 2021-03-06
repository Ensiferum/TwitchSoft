﻿using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TwitchSoft.Shared.Services.Repository.Interfaces;
using TwitchSoft.TelegramBot.MediatR.Models;

namespace TwitchSoft.TelegramBot.MediatR.Handlers
{
    public class UserMessagesHandler : AsyncRequestHandler<UserMessagesCommand>
    {
        private readonly ITelegramBotClient telegramBotClient;
        private readonly IUserRepository userRepository;
        private readonly IMessageRepository messageRepository;

        public UserMessagesHandler(
            ITelegramBotClient telegramBotClient, 
            IUserRepository userRepository, 
            IMessageRepository messageRepository)
        {
            this.telegramBotClient = telegramBotClient;
            this.userRepository = userRepository;
            this.messageRepository = messageRepository;
        }

        protected override async Task Handle(UserMessagesCommand request, CancellationToken cancellationToken)
        {
            var chatId = request.ChatId;
            var userName = request.Username;
            var skip = request.Skip;
            var userIds = await userRepository.GetUserIds(userName);
            if (!userIds.Any())
            {
                await telegramBotClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"User '{userName}' not found.",
                    parseMode: ParseMode.Html,
                    cancellationToken: cancellationToken
                );
                return;
            }

            var count = 50;
            var messages = await messageRepository.GetMessages(userIds.First().Value, skip, count);

            var replyMessages = messages.GenerateReplyMessages();
            if (!replyMessages.Any())
            {
                await telegramBotClient.SendTextMessageAsync(
                    chatId,
                    "No messages were found.",
                    cancellationToken: cancellationToken
                );
            }
            for (var i = 0; i < replyMessages.Count; i++)
            {
                var replyMessage = replyMessages[i];
                await telegramBotClient.SendTextMessageAsync(
                    chatId,
                    replyMessage,
                    parseMode: ParseMode.Html,
                    disableWebPagePreview: true,
                    replyMarkup: i == replyMessages.Count - 1
                        ? InlineUtils.GenerateNavigationMarkup(BotCommands.UserMessages, userName, count, skip, messages.Count)
                        : null,
                    cancellationToken: cancellationToken
                );
            }
        }
    }
}
