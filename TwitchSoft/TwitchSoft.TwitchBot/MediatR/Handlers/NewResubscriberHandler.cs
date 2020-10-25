using AutoMapper;
using MassTransit;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TwitchSoft.Shared.ServiceBus.Models;
using TwitchSoft.TwitchBot.MediatR.Models;

namespace TwitchSoft.TwitchBot.MediatR.Handlers
{
    public class NewResubscriberHandler : AsyncRequestHandler<NewResubscriberDto>
    {
        private readonly ISendEndpointProvider sendEndpointProvider;
        private readonly IMapper mapper;

        public NewResubscriberHandler(
            ISendEndpointProvider sendEndpointProvider,
            IMapper mapper)
        {
            this.sendEndpointProvider = sendEndpointProvider;
            this.mapper = mapper;
        }

        protected override async Task Handle(NewResubscriberDto request, CancellationToken cancellationToken)
        {
            var newSubscriber = mapper.Map<NewSubscriber>(request);

            await sendEndpointProvider.Send(newSubscriber, cancellationToken: cancellationToken);
        }
    }
}
