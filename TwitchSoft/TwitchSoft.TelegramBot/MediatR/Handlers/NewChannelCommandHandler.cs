using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TwitchSoft.Shared.Services.Repository.Interfaces;
using TwitchSoft.Shared.Services.TwitchApi;
using TwitchSoft.TelegramBot.MediatR.Models;
using static TwitchBotOrchestratorGrpc;

namespace TwitchSoft.TelegramBot.MediatR.Handlers
{
    public class NewChannelCommandHandler : AsyncRequestHandler<NewChannelCommand>
    {
        private readonly ITwitchApiService twitchApiService;
        private readonly ITelegramBotClient telegramBotClient;
        private readonly IUserRepository userRepository;
        private readonly TwitchBotOrchestratorGrpcClient twitchBotOrchestratorClient;

        public NewChannelCommandHandler(
            ITwitchApiService twitchApiService,
            ITelegramBotClient telegramBotClient,
            IUserRepository userRepository, 
            TwitchBotOrchestratorGrpcClient twitchBotOrchestratorClient)
        {
            this.twitchApiService = twitchApiService;
            this.telegramBotClient = telegramBotClient;
            this.userRepository = userRepository;
            this.twitchBotOrchestratorClient = twitchBotOrchestratorClient;
        }

        protected override async Task Handle(NewChannelCommand request, CancellationToken cancellationToken)
        {
            var chatId = request.ChatId;
            var channelName = request.ChannelName;

            var channel = await twitchApiService.GetChannelByName(channelName);
            if (channel == null)
            {
                await telegramBotClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"Channel '{channelName}' was not found.",
                        parseMode: ParseMode.Html,
                        cancellationToken: cancellationToken
                    );
                return;
            }

            var alreadyExist = await userRepository.AddChannelToTrack(channel);
            if (alreadyExist)
            {
                await telegramBotClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"Channel '{channelName}' has already been tracking.",
                        parseMode: ParseMode.Html,
                        cancellationToken: cancellationToken
                    );
            }
            else
            {
                await twitchBotOrchestratorClient.JoinChannelAsync(
                    new JoinChannelRequest { Channelname = channelName }, 
                    cancellationToken: cancellationToken);

                await telegramBotClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"Channel '{channelName}' successfully added.",
                        parseMode: ParseMode.Html,
                        cancellationToken: cancellationToken
                    );
            }
        }
    }
}
