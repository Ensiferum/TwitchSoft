using AutoMapper;
using MassTransit;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TwitchSoft.Shared.ServiceBus.Models;
using TwitchSoft.TwitchBot.MediatR.Models;

namespace TwitchSoft.TwitchBot.MediatR.Handlers
{
    public class NewUserBanHandler : AsyncRequestHandler<NewUserBanDto>
    {
        private readonly ISendEndpointProvider sendEndpointProvider;
        private readonly IMapper mapper;

        public NewUserBanHandler(
            ISendEndpointProvider sendEndpointProvider,
            IMapper mapper)
        {
            this.sendEndpointProvider = sendEndpointProvider;
            this.mapper = mapper;
        }

        protected override async Task Handle(NewUserBanDto request, CancellationToken cancellationToken)
        {
            var newSubscriber = mapper.Map<NewBan>(request.UserBan);

            await sendEndpointProvider.Send(newSubscriber, cancellationToken: cancellationToken);
        }
    }
}
