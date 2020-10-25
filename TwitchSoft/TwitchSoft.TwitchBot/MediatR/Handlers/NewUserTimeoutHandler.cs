using AutoMapper;
using MassTransit;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TwitchSoft.Shared.ServiceBus.Models;
using TwitchSoft.TwitchBot.MediatR.Models;

namespace TwitchSoft.TwitchBot.MediatR.Handlers
{
    public class NewUserTimeoutHandler : AsyncRequestHandler<NewUserTimeoutDto>
    {
        private readonly ISendEndpointProvider sendEndpointProvider;
        private readonly IMapper mapper;

        public NewUserTimeoutHandler(
            ISendEndpointProvider sendEndpointProvider,
            IMapper mapper)
        {
            this.sendEndpointProvider = sendEndpointProvider;
            this.mapper = mapper;
        }

        protected override async Task Handle(NewUserTimeoutDto request, CancellationToken cancellationToken)
        {
            var newSubscriber = mapper.Map<NewBan>(request.UserTimeout);

            await sendEndpointProvider.Send(newSubscriber, cancellationToken: cancellationToken);
        }
    }
}
