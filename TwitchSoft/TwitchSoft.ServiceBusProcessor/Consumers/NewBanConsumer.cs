using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using TwitchSoft.Shared.Database.Models;
using TwitchSoft.Shared.ServiceBus.Models;
using TwitchSoft.Shared.Services.Helpers;
using TwitchSoft.Shared.Services.Repository.Interfaces;

namespace TwitchSoft.ServiceBusProcessor.Consumers
{
    public class NewBanConsumer : IConsumer<NewBan>
    {
        private readonly ILogger<NewBanConsumer> logger;
        private readonly IRepository repository;
        private readonly IChannelsCache channelsCache;

        public NewBanConsumer(ILogger<NewBanConsumer> logger, IRepository repository, IChannelsCache channelsCache)
        {
            this.logger = logger;
            this.repository = repository;
            this.channelsCache = channelsCache;
        }

        public async Task Consume(ConsumeContext<NewBan> context)
        {
            var newBans = new List<UserBan>();

            var newBanInfos = new NewBan[] { context.Message };

            foreach (var newBanInfo in newBanInfos)
            {
                if (newBanInfo.BanType == BanType.Ban || newBanInfo.Duration >= 600)
                {
                    newBans.Add(new UserBan
                    {
                        Channel = newBanInfo.Channel,
                        BannedTime = newBanInfo.BannedTime,
                        BanType = newBanInfo.BanType,
                        Duration = newBanInfo.Duration,
                        Reason = newBanInfo.Reason,
                        UserName = newBanInfo.User.UserName,
                    });
                }
            }

            try
            {
                if (newBans.Any())
                {
                    await repository.SaveUserBansAsync(newBans.ToArray());
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "NewBan saving failed");
            }
        }
    }
}
