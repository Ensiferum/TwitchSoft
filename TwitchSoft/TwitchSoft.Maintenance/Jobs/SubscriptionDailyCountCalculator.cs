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
        private readonly IUserRepository userRepository;
        private readonly ISubscriptionRepository subscriptionRepository;
        private readonly ISubscriptionStatisticRepository subscriptionStatisticRepository;

        public SubscriptionDailyCountCalculator(
            ILogger<SubscriptionDailyCountCalculator> logger,
            IUserRepository userRepository,
            ISubscriptionRepository subscriptionRepository, 
            ISubscriptionStatisticRepository subscriptionStatisticRepository)
        {
            this.logger = logger;
            this.userRepository = userRepository;
            this.subscriptionRepository = subscriptionRepository;
            this.subscriptionStatisticRepository = subscriptionStatisticRepository;
        }

        public async Task Invoke()
        {
            logger.LogInformation($"Start executing job: {nameof(SubscriptionDailyCountCalculator)}");

            var todayUtc = DateTime.UtcNow.Date;
            var yesterdayUtc = todayUtc.AddDays(-1);

            await subscriptionStatisticRepository.CalculateStatisticsForDates(yesterdayUtc, todayUtc);

            logger.LogInformation($"End executing job: {nameof(SubscriptionDailyCountCalculator)}");
        }
    }
}
