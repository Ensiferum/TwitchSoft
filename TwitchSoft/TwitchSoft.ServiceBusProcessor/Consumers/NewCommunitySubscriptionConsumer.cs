using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TwitchSoft.Shared.Database.Models;
using TwitchSoft.Shared.ServiceBus.Models;
using TwitchSoft.Shared.Services.Repository.Interfaces;
using User = TwitchSoft.Shared.Database.Models.User;

namespace TwitchSoft.ServiceBusProcessor.Consumers
{
    public class NewCommunitySubscriptionConsumer : IConsumer<NewCommunitySubscription>
    {
        private readonly ILogger<NewCommunitySubscriptionConsumer> logger;
        private readonly IRepository repository;

        public NewCommunitySubscriptionConsumer(ILogger<NewCommunitySubscriptionConsumer> logger, IRepository repository)
        {
            this.logger = logger;
            this.repository = repository;
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
                ChannelId = comSubInfo.Channel.UserId,
                UserId = comSubInfo.User.UserId,
                SubscriptionPlan = comSubInfo.SubscriptionPlan,
                Date = comSubInfo.Date,
                GiftCount = comSubInfo.GiftCount,
            });
        }
    }
}
