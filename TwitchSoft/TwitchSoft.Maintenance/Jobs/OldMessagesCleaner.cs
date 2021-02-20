using Coravel.Invocable;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TwitchSoft.Shared.Services.Repository.Interfaces;

namespace TwitchSoft.Maintenance.Jobs
{
    public class OldMessagesCleaner : IInvocable
    {
        private readonly ILogger<OldMessagesCleaner> logger;
        private readonly IMessagesRepository messagesRepository;

        public OldMessagesCleaner(ILogger<OldMessagesCleaner> logger, IMessagesRepository messagesRepository)
        {
            this.logger = logger;
            this.messagesRepository = messagesRepository;
        }
        public async Task Invoke()
        {
            logger.LogInformation($"Start executing job: {nameof(OldMessagesCleaner)}");

            var response = await messagesRepository.RemoveOldMessages(30);

            logger.LogInformation($"Removed {response.Total} messages");

            logger.LogInformation($"End executing job: {nameof(OldMessagesCleaner)}");
        }
    }
}
