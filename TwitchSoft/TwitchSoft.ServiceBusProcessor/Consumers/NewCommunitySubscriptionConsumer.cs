﻿using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TwitchSoft.ServiceBusProcessor.Caching;
using TwitchSoft.Shared.Database.Models;
using TwitchSoft.Shared.ServiceBus.Models;
using TwitchSoft.Shared.Services.Repository.Interfaces;
using User = TwitchSoft.Shared.Database.Models.User;

namespace TwitchSoft.ServiceBusProcessor.Consumers
{
    public class NewCommunitySubscriptionConsumer : IConsumer<NewCommunitySubscription>
    {
        private readonly ILogger<NewCommunitySubscriptionConsumer> logger;
        private readonly ISubscriptionRepository subscriptionRepository;
        private readonly IUserRepository usersRepository;
        private readonly IChannelsCache channelsCache;

        public NewCommunitySubscriptionConsumer(
            ILogger<NewCommunitySubscriptionConsumer> logger,
            ISubscriptionRepository subscriptionRepository,
            IUserRepository usersRepository,
            IChannelsCache channelsCache)
        {
            this.logger = logger;
            this.subscriptionRepository = subscriptionRepository;
            this.usersRepository = usersRepository;
            this.channelsCache = channelsCache;
        }

        public async Task Consume(ConsumeContext<NewCommunitySubscription> context)
        {
            var comSubInfo = context.Message;
            await usersRepository.CreateOrUpdateUsers(new User
            {
                Username = comSubInfo.User.UserName,
                Id = comSubInfo.User.UserId
            });
            await subscriptionRepository.SaveCommunitySubscribtionAsync(new CommunitySubscription
            {
                Id = comSubInfo.Id,
                ChannelId = await channelsCache.GetChannelIdByName(comSubInfo.Channel),
                UserId = comSubInfo.User.UserId,
                SubscriptionPlan = comSubInfo.SubscriptionPlan,
                Date = comSubInfo.Date,
                GiftCount = comSubInfo.GiftCount,
            });
        }
    }
}
