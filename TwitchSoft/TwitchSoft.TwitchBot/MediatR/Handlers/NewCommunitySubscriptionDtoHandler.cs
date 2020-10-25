using AutoMapper;
using MassTransit;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TwitchSoft.Shared.ServiceBus.Models;
using TwitchSoft.TwitchBot.MediatR.Models;

namespace TwitchSoft.TwitchBot.MediatR.Handlers
{
    public class NewCommunitySubscriptionDtoHandler : AsyncRequestHandler<NewCommunitySubscriptionDto>
    {
        private readonly ISendEndpointProvider sendEndpointProvider;
        private readonly IMapper mapper;

        public NewCommunitySubscriptionDtoHandler(
            ISendEndpointProvider sendEndpointProvider,
            IMapper mapper)
        {
            this.sendEndpointProvider = sendEndpointProvider;
            this.mapper = mapper;
        }

        protected override async Task Handle(NewCommunitySubscriptionDto request, CancellationToken cancellationToken)
        {
            var newSubscriber = mapper.Map<NewCommunitySubscription>(request);

            await sendEndpointProvider.Send(newSubscriber, cancellationToken: cancellationToken);
        }
    }
}
