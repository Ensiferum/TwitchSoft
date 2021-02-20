using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TwitchSoft.Shared.Services.Repository.Interfaces;
using TwitchSoft.TwitchBot.MediatR.Models;

namespace TwitchSoft.TwitchBot.MediatR.Handlers
{
    public class SetChannelBannedHandler : AsyncRequestHandler<SetChannelBanned>
    {
        private readonly IUserRepository userRepository;

        public SetChannelBannedHandler(
            IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        protected override async Task Handle(SetChannelBanned request, CancellationToken cancellationToken)
        {
            await userRepository.SetChannelIsBanned(request.Channel, request.IsBanned);
        }
    }
}
