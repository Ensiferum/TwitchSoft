﻿using MassTransit;
using System.Threading.Tasks;
using TwitchSoft.Shared.Database.Models;
using TwitchSoft.Shared.ServiceBus.Models;
using TwitchSoft.Shared.Services.Helpers;
using TwitchSoft.Shared.Services.Repository.Interfaces;
using User = TwitchSoft.Shared.Database.Models.User;

namespace TwitchSoft.ServiceBusProcessor.Consumers
{
    public class NewTwitchChannelMessageConsumer : IConsumer<NewTwitchChannelMessage>
    {
        private readonly IRepository repository;
        private readonly IChannelsCache channelsCache;

        public NewTwitchChannelMessageConsumer(IRepository repository, IChannelsCache channelsCache)
        {
            this.repository = repository;
            this.channelsCache = channelsCache;
        }

        public async Task Consume(ConsumeContext<NewTwitchChannelMessage> context)
        {
            var chatMessage = context.Message;
            await repository.CreateOrUpdateUser(new User
            {
                Username = chatMessage.User.UserName,
                Id = chatMessage.User.UserId
            });
            await repository.SaveMessagesAsync(new ChatMessage
            {
                Id = chatMessage.Id,
                ChannelId = await channelsCache.GetChannelIdByName(chatMessage.Channel),
                UserId = chatMessage.User.UserId,
                IsBroadcaster = chatMessage.IsBroadcaster,
                IsModerator = chatMessage.IsModerator,
                IsSubscriber = chatMessage.IsSubscriber,
                Message = chatMessage.Message,
                PostedTime = chatMessage.PostedTime,
                UserType = chatMessage.UserType,
            });
        }
    }
}
