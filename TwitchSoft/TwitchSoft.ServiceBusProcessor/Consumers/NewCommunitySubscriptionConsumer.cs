using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TwitchSoft.Shared.Database.Models;
using TwitchSoft.Shared.ServiceBus.Models;
using TwitchSoft.Shared.Services.Helpers;
using TwitchSoft.Shared.Services.Repository.Interfaces;
using User = TwitchSoft.Shared.Database.Models.User;

namespace TwitchSoft.ServiceBusProcessor.Consumers
{
    public class NewCommunitySubscriptionConsumer : IConsumer<NewCommunitySubscription>
    {
        private readonly ILogger<NewCommunitySubscriptionConsumer> logger;
        private readonly IRepository repository;
        private readonly IChannelsCache channelsCache;

        public NewCommunitySubscriptionConsumer(
            ILogger<NewCommunitySubscriptionConsumer> logger, 
            IRepository repository,
            IChannelsCache channelsCache)
        {
            this.logger = logger;
            this.repository = repository;
            this.channelsCache = channelsCache;
        }

        public async Task Consume(ConsumeContext<NewCommunitySubscription> context)
        {
            var comSubInfo = context.Message;
            await repository.CreateOrUpdateUser(new User
            {
                Username = comSubInfo.User.UserName,
                Id = comSubInfo.User.UserId
            });
            await repository.SaveCommunitySubscribtionAsync(new CommunitySubscription
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
