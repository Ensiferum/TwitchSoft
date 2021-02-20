using MassTransit;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchSoft.Shared.Database.Models;
using TwitchSoft.Shared.ServiceBus.Models;
using TwitchSoft.Shared.Services.Helpers;
using TwitchSoft.Shared.Services.Repository.Interfaces;
using User = TwitchSoft.Shared.Database.Models.User;

namespace TwitchSoft.ServiceBusProcessor.Consumers
{
    public class NewSubscriberConsumer : IConsumer<NewSubscriber>
    {
        private readonly ISubscriptionRepository subscriptionRepository;
        private readonly IUserRepository userRepository;
        private readonly IChannelsCache channelsCache;

        public NewSubscriberConsumer(
            ISubscriptionRepository subscriptionRepository,
            IUserRepository userRepository,
            IChannelsCache channelsCache
            )
        {
            this.subscriptionRepository = subscriptionRepository;
            this.channelsCache = channelsCache;
            this.userRepository = userRepository;
        }

        public async Task Consume(ConsumeContext<NewSubscriber> context)
        {
            var subInfo = context.Message;
            var users = new List<User>() { new User
            {
                Username = subInfo.User.UserName,
                Id = subInfo.User.UserId
            }};
            
            if (subInfo.GiftedBy != null)
            {
                users.Add(new User
                {
                    Username = subInfo.GiftedBy.UserName,
                    Id = subInfo.GiftedBy.UserId
                });
            }
            await userRepository.CreateOrUpdateUsers(users.ToArray());
            await subscriptionRepository.SaveSubscriberAsync(new Subscription
            {
                Id = subInfo.Id,
                ChannelId = await channelsCache.GetChannelIdByName(subInfo.Channel),
                UserId = subInfo.User.UserId,
                Months = subInfo.Months,
                SubscribedTime = subInfo.SubscribedTime,
                SubscriptionPlan = subInfo.SubscriptionPlan,
                UserType = subInfo.UserType,
                GiftedBy = subInfo.GiftedBy?.UserId
            });
        }
    }
}
