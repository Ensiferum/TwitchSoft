using MassTransit;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using TwitchSoft.ServiceBusProcessor.Caching;
using TwitchSoft.Shared;
using TwitchSoft.Shared.Extensions;
using TwitchSoft.Shared.ServiceBus.Models;
using TwitchSoft.Shared.Services.Models;
using TwitchSoft.Shared.Services.Repository.Interfaces;
using static TelegramBotGrpc;
using ChatMessageES = TwitchSoft.Shared.ElasticSearch.Models.ChatMessage;

namespace TwitchSoft.ServiceBusProcessor.Consumers
{
    public class NewTwitchChannelMessageConsumer : IConsumer<NewTwitchChannelMessage>
    {
        private readonly IChannelsCache channelsCache;
        private readonly IMessageRepository messageRepository;
        private readonly TelegramBotGrpcClient telegramBotClient;
        private readonly string rootUserChatId;

        public NewTwitchChannelMessageConsumer(
            IChannelsCache channelsCache,
            IMessageRepository messageRepository,
            TelegramBotGrpcClient telegramBotClient,
            IConfiguration config)
        {
            this.channelsCache = channelsCache;
            this.messageRepository = messageRepository;
            this.telegramBotClient = telegramBotClient;

            rootUserChatId = config.GetValue<string>("JobConfigs:RootUserChatId");
        }

        public async Task Consume(ConsumeContext<NewTwitchChannelMessage> context)
        {
            var chatMessage = context.Message;

            var channelId = await channelsCache.GetChannelIdByName(chatMessage.Channel);

            //ToDo: extract logic and make configurable via bot
            if (chatMessage.User.UserId == Constants.MadTwitchId)
            {
                var messageModel = new ChatMessageModelForDisplaying()
                {
                    Channel = chatMessage.Channel,
                    Message = chatMessage.Message,
                    PostedTime = chatMessage.PostedTime,
                    UserName = chatMessage.User.UserName,
                };

                await telegramBotClient.SendMessageAsync(new SendMessageRequest
                {
                    ChatId = rootUserChatId,
                    MessageText = messageModel.ToDisplayFormat()
                });
            }

            //ToDo: notify about mentions
            if (chatMessage.Message.Contains($"@{Constants.EnsthorTwitchName}", System.StringComparison.OrdinalIgnoreCase))
            {
                var messageModel = new ChatMessageModelForDisplaying()
                {
                    Channel = chatMessage.Channel,
                    Message = chatMessage.Message,
                    PostedTime = chatMessage.PostedTime,
                    UserName = chatMessage.User.UserName,
                };

                await telegramBotClient.SendMessageAsync(new SendMessageRequest
                {
                    ChatId = rootUserChatId,
                    MessageText = messageModel.ToDisplayFormat()
                });
            }

            var chatMessageES = new ChatMessageES
            {
                Id = chatMessage.Id,
                ChannelId = channelId,
                ChannelName = chatMessage.Channel,
                UserId = chatMessage.User.UserId,
                UserName = chatMessage.User.UserName,
                IsBroadcaster = chatMessage.IsBroadcaster,
                IsModerator = chatMessage.IsModerator,
                IsSubscriber = chatMessage.IsSubscriber,
                Message = chatMessage.Message,
                PostedTime = chatMessage.PostedTime,
            };

            await messageRepository.SaveMessage(chatMessageES);
        }
    }
}
