using AutoMapper;
using MassTransit;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Client.Interfaces;
using TwitchSoft.Shared.ServiceBus.Models;
using TwitchSoft.TwitchBot.ChatPlugins;
using TwitchSoft.TwitchBot.MediatR.Models;

namespace TwitchSoft.TwitchBot.MediatR.Handlers
{
    public class NewMessageHandler : AsyncRequestHandler<NewChatMessageDto>
    {
        private readonly ISendEndpointProvider sendEndpointProvider;
        private readonly IEnumerable<IChatPlugin> chatPlugins;
        private readonly ITwitchClient twitchClient;
        private readonly IMapper mapper;

        public NewMessageHandler(
            ISendEndpointProvider sendEndpointProvider,
            IEnumerable<IChatPlugin> chatPlugins,
            ITwitchClient twitchClient,
            IMapper mapper)
        {
            this.sendEndpointProvider = sendEndpointProvider;
            this.chatPlugins = chatPlugins;
            this.twitchClient = twitchClient;
            this.mapper = mapper;
        }
        protected override async Task Handle(NewChatMessageDto request, CancellationToken cancellationToken)
        {
            var chatMessage = request.ChatMessage;

            var cmDto = mapper.Map<NewTwitchChannelMessage>(chatMessage);
            
            await sendEndpointProvider.Send(cmDto, cancellationToken: cancellationToken);

            foreach (var plugin in chatPlugins)
            {
                await plugin.ProcessMessage(chatMessage, twitchClient);
            }
        }
    }
}
