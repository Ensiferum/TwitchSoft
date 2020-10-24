using MassTransit;
using Nest;
using System.Threading.Tasks;
using TwitchSoft.Shared.ServiceBus.Models;
using TwitchSoft.Shared.Services.Helpers;
using TwitchSoft.Shared.Services.Repository.Interfaces;
using ChatMessageES = TwitchSoft.Shared.ElasticSearch.Models.ChatMessage;

namespace TwitchSoft.ServiceBusProcessor.Consumers
{
    public class NewTwitchChannelMessageConsumer : IConsumer<NewTwitchChannelMessage>
    {
        private readonly IUsersRepository usersRepository;
        private readonly IChannelsCache channelsCache;
        private readonly IElasticClient elasticClient;

        public NewTwitchChannelMessageConsumer(
            IUsersRepository usersRepository, 
            IChannelsCache channelsCache,
            IElasticClient elasticClient)
        {
            this.usersRepository = usersRepository;
            this.channelsCache = channelsCache;
            this.elasticClient = elasticClient;
        }

        public async Task Consume(ConsumeContext<NewTwitchChannelMessage> context)
        {
            var chatMessage = context.Message;

            var channelId = await channelsCache.GetChannelIdByName(chatMessage.Channel);

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

            await elasticClient.IndexManyAsync(new[] { chatMessageES });
        }
    }
}
