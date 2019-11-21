using Coravel.Invocable;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using TwitchSoft.Shared.Database;
using TwitchSoft.Shared.Services.TwitchApi;
using static TwitchBotGrpc;

namespace TwitchSoft.Maintenance.Jobs
{
    public class TopChannelsJoin : IInvocable
    {
        private readonly ILogger<TopChannelsJoin> logger;
        private readonly ITwitchApiService twitchApiService;
        private readonly TwitchDbContext twitchDbContext;

        public TopChannelsJoin(
            ILogger<TopChannelsJoin> logger,
            ITwitchApiService twitchApiService,
            TwitchDbContext twitchDbContext)
        {
            this.logger = logger;
            this.twitchApiService = twitchApiService;
            this.twitchDbContext = twitchDbContext;
        }
        public async Task Invoke()
        {
            logger.LogInformation($"Start executing job: {nameof(TopChannelsJoin)}");
            var streams = await twitchApiService.GetTopStreams();

            var channelIds = streams.Select(_ => uint.Parse(_.UserId));

            var existingUsers = await twitchDbContext.Users.Where(_ => channelIds.Contains(_.Id)).ToListAsync();
            existingUsers.ForEach(_ =>
            {
                _.Username = _.Username.ToLower();
                _.JoinChannel = true;
            });

            twitchDbContext.Users.UpdateRange(existingUsers);

            var notExistingUsers = streams.Where(_ => !existingUsers.Select(u => u.Id).Contains(uint.Parse(_.UserId)));


            await twitchDbContext.Users.AddRangeAsync(notExistingUsers.Select(f => new Shared.Database.Models.User
            {
                Id = uint.Parse(f.UserId),
                Username = f.UserName.ToLower(),
                JoinChannel = true,
            }));
            await twitchDbContext.SaveChangesAsync();

            Channel grpcChannel = new Channel("twitchbot", 80, ChannelCredentials.Insecure);
            var client = new TwitchBotGrpcClient(grpcChannel);
            await client.RefreshChannelsAsync(new Empty());
            logger.LogInformation($"End executing job: {nameof(TopChannelsJoin)}");
        }
    }
}
