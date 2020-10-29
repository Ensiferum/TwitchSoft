using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TwitchSoft.Shared.Services.Repository.Interfaces;
using TwitchSoft.TwitchBot.MediatR.Models;

namespace TwitchSoft.TwitchBot.MediatR.Handlers
{
    public class SetChannelBannedHandler : AsyncRequestHandler<SetChannelBanned>
    {
        private readonly IUsersRepository usersRepository;

        public SetChannelBannedHandler(
            IUsersRepository usersRepository)
        {
            this.usersRepository = usersRepository;
        }

        protected override async Task Handle(SetChannelBanned request, CancellationToken cancellationToken)
        {
            await usersRepository.SetChannelIsBanned(request.Channel, request.IsBanned);
        }
    }
}
