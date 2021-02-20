using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TwitchSoft.Shared.Services.Repository.Interfaces;
using TwitchSoft.TelegramBot.MediatR.Models;

namespace TwitchSoft.TelegramBot.MediatR.Handlers
{
    public class SearchTextCommandHandler : AsyncRequestHandler<SearchTextCommand>
    {
        private readonly ITelegramBotClient telegramBotClient;
        private readonly IMessageRepository messageRepository;

        public SearchTextCommandHandler(ITelegramBotClient telegramBotClient, IMessageRepository messageRepository)
        {
            this.telegramBotClient = telegramBotClient;
            this.messageRepository = messageRepository;
        }

        protected override async Task Handle(SearchTextCommand request, CancellationToken cancellationToken)
        {
            var chatId = request.ChatId;
            var searchText = request.SearchText;
            var skip = request.Skip;

            var count = 50;
            var messages = await messageRepository.SearchMessages(searchText.ToLower(), skip, count);

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
                        ? InlineUtils.GenerateNavigationMarkup(BotCommands.SearchText, searchText, count, skip, messages.Count)
                        : null,
                    cancellationToken: cancellationToken
                );
            }
        }
    }
}
