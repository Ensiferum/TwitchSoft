using Coravel.Invocable;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TwitchSoft.Shared.Services.Repository.Interfaces;

namespace TwitchSoft.Maintenance.Jobs
{
    public class OldMessagesCleaner : IInvocable
    {
        private readonly ILogger<OldMessagesCleaner> logger;
        private readonly IMessageRepository messageRepository;

        public OldMessagesCleaner(ILogger<OldMessagesCleaner> logger, IMessageRepository messageRepository)
        {
            this.logger = logger;
            this.messageRepository = messageRepository;
        }
        public async Task Invoke()
        {
            logger.LogInformation($"Start executing job: {nameof(OldMessagesCleaner)}");

            var response = await messageRepository.RemoveOldMessages(30);

            logger.LogInformation($"Removed {response.Total} messages");

            logger.LogInformation($"End executing job: {nameof(OldMessagesCleaner)}");
        }
    }
}
