using Coravel.Invocable;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TwitchSoft.Shared.Services.Repository.Interfaces;

namespace TwitchSoft.Maintenance.Jobs
{
    public class OldMessagesCleaner : IInvocable
    {
        private readonly ILogger<OldMessagesCleaner> logger;
        private readonly IRepository repository;
        private int OldDaysInterval { get; }

        public OldMessagesCleaner(
            ILogger<OldMessagesCleaner> logger,
            IRepository repository,
            IConfiguration config)
        {
            var interval = config.GetValue<int>("JobConfigs:OldMessagesIntervalInDays");
            OldDaysInterval = interval > 5 ? interval : 5;
            this.logger = logger;
            this.repository = repository;
        }
        public Task Invoke()
        {
            logger.LogInformation($"Start executing job: {nameof(OldMessagesCleaner)}");
            //await repository.RemoveMessagesPriorTo(DateTime.UtcNow.AddDays(-OldDaysInterval));
            logger.LogInformation($"End executing job: {nameof(OldMessagesCleaner)}");
            return Task.CompletedTask;
        }
    }
}
