using MassTransit;
using Nest;
using System.Threading.Tasks;
using TwitchSoft.Shared.ServiceBus.Models;
using TwitchSoft.Shared.Services.Helpers;
using TwitchSoft.Shared.Services.Repository.Interfaces;
using User = TwitchSoft.Shared.Database.Models.User;
using ChatMessageES = TwitchSoft.Shared.ElasticSearch.Models.ChatMessage;
using System.Linq;

namespace TwitchSoft.ServiceBusProcessor.Consumers
{
    public class NewTwitchChannelMessageConsumer : IConsumer<NewTwitchChannelMessage>
    {
        private readonly IRepository repository;
        private readonly IChannelsCache channelsCache;
        private readonly IElasticClient elasticClient;

        public NewTwitchChannelMessageConsumer(
            IRepository repository, 
            IChannelsCache channelsCache,
            IElasticClient elasticClient)
        {
            this.repository = repository;
            this.channelsCache = channelsCache;
            this.elasticClient = elasticClient;
        }

        public async Task Consume(ConsumeContext<NewTwitchChannelMessage> context)
        {
            var chatMessages = new NewTwitchChannelMessage[] { context.Message };
            await repository.CreateOrUpdateUsers(chatMessages.Select(chatMessage => new User
            {
                Username = chatMessage.User.UserName,
                Id = chatMessage.User.UserId
            }).ToArray());

            var channels = await channelsCache.GetChannelsByNames(chatMessages.Select(_ => _.Channel).ToArray());

            var chatMessagesES = chatMessages.Select(chatMessage => new ChatMessageES
            {
                Id = chatMessage.Id,
                ChannelId = channels[chatMessage.Channel],
                ChannelName = chatMessage.Channel,
                UserId = chatMessage.User.UserId,
                UserName = chatMessage.User.UserName,
                IsBroadcaster = chatMessage.IsBroadcaster,
                IsModerator = chatMessage.IsModerator,
                IsSubscriber = chatMessage.IsSubscriber,
                Message = chatMessage.Message,
                PostedTime = chatMessage.PostedTime,
            });

            await elasticClient.IndexManyAsync(chatMessagesES);
        }
    }
}
