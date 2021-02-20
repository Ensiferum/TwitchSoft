using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TwitchSoft.Shared.Services.Repository.Interfaces;
using TwitchSoft.TelegramBot.MediatR.Models;

namespace TwitchSoft.TelegramBot.MediatR.Handlers
{
    public class UserMessagesDigestHandler : AsyncRequestHandler<UserMessagesDigest>
    {
        private readonly IUserRepository userRepository;
        private readonly IMessageRepository messageRepository;
        private readonly ITelegramBotClient telegramBotClient;

        public UserMessagesDigestHandler(
            IUserRepository userRepository,
            IMessageRepository messageRepository, 
            ITelegramBotClient telegramBotClient)
        {
            this.userRepository = userRepository;
            this.messageRepository = messageRepository;
            this.telegramBotClient = telegramBotClient;
        }
        protected override async Task Handle(UserMessagesDigest request, CancellationToken cancellationToken)
        {
            var userIds = await userRepository.GetUserIds(request.Username);

            var count = 50;
            var messages = await messageRepository.GetMessages(userIds.First().Value, DateTime.UtcNow.AddHours(-12), count);

            var replyMessages = messages.GenerateReplyMessages();
            for (var i = 0; i < replyMessages.Count; i++)
            {
                var replyMessage = replyMessages[i];
                await telegramBotClient.SendTextMessageAsync(
                    request.ChatId,
                    replyMessage,
                    parseMode: ParseMode.Html,
                    disableWebPagePreview: true,
                    replyMarkup: i == replyMessages.Count - 1
                        ? InlineUtils.GenerateNavigationMarkup(BotCommands.UserMessages, request.Username, count, 0, messages.Count)
                        : null,
                    cancellationToken: cancellationToken
                );
            }
        }
    }
}
