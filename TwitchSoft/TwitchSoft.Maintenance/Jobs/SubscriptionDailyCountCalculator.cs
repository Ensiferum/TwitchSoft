using Coravel.Invocable;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TwitchSoft.Shared.Services.Repository.Interfaces;

namespace TwitchSoft.Maintenance.Jobs
{
    public class SubscriptionDailyCountCalculator : IInvocable
    {
        private readonly ILogger<SubscriptionDailyCountCalculator> logger;
        private readonly IUsersRepository usersRepository;
        private readonly ISubscriptionsRepository subscriptionsRepository;
        private readonly ISubscriptionStatisticsRepository subscriptionStatisticsRepository;

        public SubscriptionDailyCountCalculator(
            ILogger<SubscriptionDailyCountCalculator> logger,
            IUsersRepository usersRepository,
            ISubscriptionsRepository subscriptionsRepository, 
            ISubscriptionStatisticsRepository subscriptionStatisticsRepository)
        {
            this.logger = logger;
            this.usersRepository = usersRepository;
            this.subscriptionsRepository = subscriptionsRepository;
            this.subscriptionStatisticsRepository = subscriptionStatisticsRepository;
        }

        public async Task Invoke()
        {
            logger.LogInformation($"Start executing job: {nameof(SubscriptionDailyCountCalculator)}");

            var todayUtc = DateTime.UtcNow.Date;
            var yesterdayUtc = todayUtc.AddDays(-1);

            await subscriptionStatisticsRepository.CalculateStatisticsForDates(yesterdayUtc, todayUtc);

            logger.LogInformation($"End executing job: {nameof(SubscriptionDailyCountCalculator)}");
        }
    }
}
