using System;
using System.Threading.Tasks;
using TwitchSoft.Shared.Database.Models;

namespace TwitchSoft.Shared.Services.Repository.Interfaces
{
    public interface ISubscriptionStatisticsRepository
    {
        Task SaveStatistic(SubscriptionStatistic subStat);
        Task CalculateStatisticsForDates(DateTime fromDate, DateTime toDate);
    }
}
