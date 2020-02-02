﻿using System.Collections.Generic;
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
            var userNamesToLoad = newBanInfos
                .Select(_ => _.User.UserName)
                .ToArray();

            var usersMap = await repository.GetUserIds(userNamesToLoad);

            foreach (var newBanInfo in newBanInfos)
            {
                if (usersMap.TryGetValue(newBanInfo.User.UserName, out var userId))
                {
                    newBans.Add(new UserBan
                    {
                        ChannelId = await channelsCache.GetChannelIdByName(newBanInfo.Channel),
                        BannedTime = newBanInfo.BannedTime,
                        BanType = newBanInfo.BanType,
                        Duration = newBanInfo.Duration,
                        Reason = newBanInfo.Reason,
                        UserId = userId,
                    });
                }
            }

            try
            {
                await repository.SaveUserBansAsync(newBans.ToArray());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "NewBan saving failed");
            }
        }
    }
}
