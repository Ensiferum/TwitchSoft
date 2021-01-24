using Coravel.Invocable;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TwitchSoft.Shared.ElasticSearch.Interfaces;

namespace TwitchSoft.Maintenance.Jobs
{
    public class OldMessagesCleaner : IInvocable
    {
        private readonly ILogger<OldMessagesCleaner> logger;
        private readonly IESService eSService;

        public OldMessagesCleaner(ILogger<OldMessagesCleaner> logger, IESService eSService)
        {
            this.logger = logger;
            this.eSService = eSService;
        }
        public async Task Invoke()
        {
            logger.LogInformation($"Start executing job: {nameof(OldMessagesCleaner)}");

            var response = await eSService.RemoveOldMessages(30);

            logger.LogInformation($"Cleaned {response.Total} messages");

            logger.LogInformation($"End executing job: {nameof(OldMessagesCleaner)}");
        }
    }
}
