﻿using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using TwitchSoft.Shared.Database.Models;
using TwitchSoft.Shared.ServiceBus.Models;
using TwitchSoft.Shared.Services.Repository.Interfaces;

namespace TwitchSoft.ServiceBusProcessor.Consumers
{
    public class NewBanConsumer : IConsumer<NewBan>
    {
        private readonly ILogger<NewBanConsumer> logger;
        private readonly IRepository repository;

        public NewBanConsumer(ILogger<NewBanConsumer> logger, IRepository repository)
        {
            this.logger = logger;
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<NewBan> context)
        {
            var newBanInfo = context.Message;
            if (newBanInfo.User.UserId == 0)
            {
                newBanInfo.User.UserId = await repository.GetUserId(newBanInfo.User.UserName);
                if (newBanInfo.User.UserId == 0)
                {
                    logger.LogInformation($"User: {newBanInfo.User.UserName} was not found in db");
                    return;
                }
            }
            await repository.SaveUserBanAsync(new UserBan
            {
                ChannelId = newBanInfo.Channel.UserId,
                BannedTime = newBanInfo.BannedTime,
                BanType = newBanInfo.BanType,
                Duration = newBanInfo.Duration,
                Reason = newBanInfo.Reason,
                UserId = newBanInfo.User.UserId,
            });
        }
    }
}