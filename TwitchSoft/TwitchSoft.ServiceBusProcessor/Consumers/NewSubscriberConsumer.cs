using MassTransit;
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
        private readonly IRepository repository;
        private readonly IChannelsCache channelsCache;

        public NewSubscriberConsumer(IRepository repository, IChannelsCache channelsCache)
        {
            this.repository = repository;
            this.channelsCache = channelsCache;
        }

        public async Task Consume(ConsumeContext<NewSubscriber> context)
        {
            var subInfo = context.Message;
            await repository.CreateOrUpdateUser(new User
            {
                Username = subInfo.User.UserName,
                Id = subInfo.User.UserId
            });
            if (subInfo.GiftedBy != null)
            {
                await repository.CreateOrUpdateUser(new User
                {
                    Username = subInfo.GiftedBy.UserName,
                    Id = subInfo.GiftedBy.UserId
                });
            }
            await repository.SaveSubscriberAsync(new Subscription
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
