using Coravel.Invocable;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TwitchSoft.Shared.Database.Models;
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

            var channels = await usersRepository.GetChannelsToTrack();

            foreach (var channel in channels)
            {
                var yesterdayDateUtc = DateTime.UtcNow.AddDays(-1).Date;
                var dailySubCount = await subscriptionsRepository.GetSubscribersCountOnDay(channel.Id, yesterdayDateUtc);

                await subscriptionStatisticsRepository.SaveStatistic(new SubscriptionStatistic
                {
                    UserId = channel.Id,
                    Date = yesterdayDateUtc,
                    Count = dailySubCount
                });
            }

            logger.LogInformation($"End executing job: {nameof(SubscriptionDailyCountCalculator)}");
        }
    }
}
