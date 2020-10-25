using AutoMapper;
using FluentAssertions;
using System;
using TwitchLib.Client.Models;
using TwitchSoft.Shared.ServiceBus.Models;
using TwitchSoft.TwitchBot;
using TwitchSoft.TwitchBot.MediatR.Models;
using Xunit;

namespace TwitchSoft.Tests
{
    public class TwitchBotProfileTests
    {
        private MapperConfiguration Configuration;

        public TwitchBotProfileTests()
        {
            this.Configuration = new MapperConfiguration(cfg => {
                cfg.AddProfile<TwitchBotProfile>();
            });
        }

        [Fact]
        public void AutoMapper_Configuration_IsValid()
        {
            Configuration.AssertConfigurationIsValid();
        }

        [Fact]
        public void AutoMapper_ConvertFrom_ChatMessage_To_NewTwitchChannelMessage_IsValid()
        {
            var mapper = Configuration.CreateMapper();

            var guid = Guid.NewGuid();

            var sourceObject = new TwitchLib.Client.Models.ChatMessage(
                    botUsername: "",
                    userId: "123456",
                    userName: "testUserName",
                    displayName: "testDisplayName",
                    colorHex: "",
                    color: new System.Drawing.Color(),
                    emoteSet: new TwitchLib.Client.Models.EmoteSet("", ""),
                    message: "testMessage",
                    userType: TwitchLib.Client.Enums.UserType.Moderator,
                    channel: "testChannel",
                    id: guid.ToString(),
                    isSubscriber: true,
                    subscribedMonthCount: 25,
                    roomId: "",
                    isTurbo: false,
                    isModerator: true,
                    isMe: false,
                    isBroadcaster: false,
                    isVip: false,
                    isPartner: false,
                    isStaff: false,
                    noisy: TwitchLib.Client.Enums.Noisy.NotSet,
                    rawIrcMessage: "",
                    emoteReplacedMessage: "",
                    badges: null,
                    cheerBadge: null,
                    bits: 120,
                    bitsInDollars: 121);

            var resultObject = mapper.Map<NewTwitchChannelMessage>(sourceObject);

            var expectedObject = new NewTwitchChannelMessage() {
                Channel = "testChannel",
                IsBroadcaster = false,
                IsModerator = true,
                IsSubscriber = true,
                Id = guid,
                Message = "testMessage",
                PostedTime = DateTime.UtcNow,
                User = new User
                {
                    UserId = 123456,
                    UserName = "testUserName",
                },
                UserType = Shared.Database.Models.UserType.Moderator,
            };
            resultObject.Should().BeEquivalentTo(expectedObject, options => options
                .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(100)))
                .WhenTypeIs<DateTime>()
            );
        }

        [Fact]
        public void AutoMapper_ConvertFrom_Subscriber_To_NewSubscriber_IsValid()
        {
            var mapper = Configuration.CreateMapper();

            var guid = Guid.NewGuid();

            var sourceObject = new TwitchLib.Client.Models.Subscriber(
                badges: null,
                badgeInfo: null,
                colorHex: null,
                color: new System.Drawing.Color(),
                displayName: "testDisplayName",
                emoteSet: "",
                id: guid.ToString(),
                login: "testLogin",
                systemMessage: "",
                msgId: "",
                msgParamCumulativeMonths: "",
                msgParamStreakMonths: "",
                msgParamShouldShareStreak: true,
                systemMessageParsed: "",
                resubMessage: "",
                subscriptionPlan: TwitchLib.Client.Enums.SubscriptionPlan.Tier2,
                subscriptionPlanName: "",
                roomId: "",
                userId: "123456",
                isModerator: true,
                isTurbo: false,
                isSubscriber: true,
                isPartner: false,
                tmiSentTs: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
                userType: TwitchLib.Client.Enums.UserType.Staff,
                rawIrc: "",
                channel: "testChannel"
             );

            var resultObject = mapper.Map<NewSubscriber>(sourceObject);

            var expectedObject = new NewSubscriber()
            {
                Channel = "testChannel",
                Months = 0,
                SubscribedTime = DateTime.UtcNow,
                SubscriptionPlan = Shared.Database.Models.SubscriptionPlan.Tier2,
                Id = guid,
                User = new User
                {
                    UserId = 123456,
                    UserName = "testLogin",
                },
                UserType = Shared.Database.Models.UserType.Staff,
            };
            resultObject.Should().BeEquivalentTo(expectedObject, options => options
                .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(100)))
                .WhenTypeIs<DateTime>()
            );
        }

        [Fact]
        public void AutoMapper_ConvertFrom_NewResubscriberDto_To_NewSubscriber_IsValid()
        {
            var mapper = Configuration.CreateMapper();

            var guid = Guid.NewGuid();

            var sourceObject = new NewResubscriberDto
            {
                Channel = "testChannelOrigin",
                ReSubscriber = new TwitchLib.Client.Models.ReSubscriber(
                   badges: null,
                   badgeInfo: null,
                   colorHex: null,
                   color: new System.Drawing.Color(),
                   displayName: "testDisplayName",
                   emoteSet: "",
                   id: guid.ToString(),
                   login: "testLogin",
                   systemMessage: "",
                   msgId: "",
                   msgParamCumulativeMonths: "23",
                   msgParamStreakMonths: "",
                   msgParamShouldShareStreak: true,
                   systemMessageParsed: "",
                   resubMessage: "",
                   subscriptionPlan: TwitchLib.Client.Enums.SubscriptionPlan.Tier2,
                   subscriptionPlanName: "",
                   roomId: "",
                   userId: "123456",
                   isModerator: true,
                   isTurbo: false,
                   isSubscriber: true,
                   isPartner: false,
                   tmiSentTs: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
                   userType: TwitchLib.Client.Enums.UserType.Staff,
                   rawIrc: "",
                   channel: "testChannel",
                   months: 23
                )
            };


            var resultObject = mapper.Map<NewSubscriber>(sourceObject);

            var expectedObject = new NewSubscriber()
            {
                Channel = "testChannelOrigin",
                Months = 23,
                SubscribedTime = DateTime.UtcNow,
                SubscriptionPlan = Shared.Database.Models.SubscriptionPlan.Tier2,
                Id = guid,
                User = new User
                {
                    UserId = 123456,
                    UserName = "testLogin",
                },
                UserType = Shared.Database.Models.UserType.Staff,
            };
            resultObject.Should().BeEquivalentTo(expectedObject, options => options
                .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(100)))
                .WhenTypeIs<DateTime>()
            );
        }

        [Fact]
        public void AutoMapper_ConvertFrom_NewGiftedSubscriptionDto_To_NewSubscriber_IsValid()
        {
            var mapper = Configuration.CreateMapper();

            var guid = Guid.NewGuid();

            var sourceObject = new NewGiftedSubscriptionDto
            {
                Channel = "testChannel",
                GiftedSubscription = new TwitchLib.Client.Models.GiftedSubscription(
                    badges: null,
                    badgeInfo: null,
                    color: "",
                    displayName: "testDisplayName",
                    emotes: "",
                    id: guid.ToString(),
                    login: "testLogin",
                    isModerator: true,
                    msgId: "",
                    msgParamMonths: "10",
                    msgParamRecipientDisplayName: "targetDisplayName",
                    msgParamRecipientId: "111111",
                    msgParamRecipientUserName: "targetUserName",
                    msgParamSubPlanName: "Prime",
                    msgMultiMonthDuration: "11",
                    msgParamSubPlan: TwitchLib.Client.Enums.SubscriptionPlan.Prime,
                    roomId: "",
                    isSubscriber: true,
                    systemMsg: "",
                    systemMsgParsed: "",
                    tmiSentTs: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
                    isTurbo: false,
                    userType: TwitchLib.Client.Enums.UserType.Staff,
                    userId: "123456"
                )
            };

            var resultObject = mapper.Map<NewSubscriber>(sourceObject);

            var expectedObject = new NewSubscriber()
            {
                Channel = "testChannel",
                Months = 10,
                SubscribedTime = DateTime.UtcNow,
                SubscriptionPlan = Shared.Database.Models.SubscriptionPlan.Prime,
                Id = guid,
                User = new User
                {
                    UserId = 111111,
                    UserName = "targetUserName",
                },
                GiftedBy = new User
                {
                    UserId = 123456,
                    UserName = "testLogin",
                },
                UserType = Shared.Database.Models.UserType.Staff,
            };
            resultObject.Should().BeEquivalentTo(expectedObject, options => options
                .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(100)))
                .WhenTypeIs<DateTime>()
            );
        }

        [Fact]
        public void AutoMapper_ConvertFrom_NewCommunitySubscriptionDto_To_NewCommunitySubscription_IsValid()
        {
            var mapper = Configuration.CreateMapper();

            var guid = Guid.NewGuid();

            var ircMessage = new TwitchLib.Client.Models.Internal.IrcMessage(
                new TwitchLib.Client.Enums.Internal.IrcCommand(),
                new string[] { },
                "",
                new System.Collections.Generic.Dictionary<string, string>());

            var sourceObject = new NewCommunitySubscriptionDto
            {
                Channel = "testChannel",
                CommunitySubscription = new TwitchLib.Client.Models.CommunitySubscription(ircMessage)
                {
                    UserId = "111111",
                    Login = "testUserName",
                    Id = guid.ToString(),
                    TmiSentTs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
                    MsgParamSubPlan = TwitchLib.Client.Enums.SubscriptionPlan.Tier3,
                    MsgParamMassGiftCount = 15,
                }
            };

            var resultObject = mapper.Map<NewCommunitySubscription>(sourceObject);

            var expectedObject = new NewCommunitySubscription()
            {
                Channel = "testChannel",
                SubscriptionPlan = Shared.Database.Models.SubscriptionPlan.Tier3,
                Id = guid,
                User = new User
                {
                    UserId = 111111,
                    UserName = "testUserName",
                },
                Date = DateTime.UtcNow,
                GiftCount = 15,
            };
            resultObject.Should().BeEquivalentTo(expectedObject, options => options
                .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(100)))
                .WhenTypeIs<DateTime>()
            );
        }

        [Fact]
        public void AutoMapper_ConvertFrom_UserBan_To_NewBan_IsValid()
        {
            var mapper = Configuration.CreateMapper();

            var sourceObject = new UserBan("testChannel", "testUserName", "testReason", "", "111111");

            var resultObject = mapper.Map<NewBan>(sourceObject);

            var expectedObject = new NewBan()
            {
                Channel = "testChannel",
                Reason = "testReason",
                BanType = Shared.Database.Models.BanType.Ban,
                BannedTime = DateTime.UtcNow,
                User = new User
                {
                    UserId = 111111,
                    UserName = "testUserName",
                },
            };
            resultObject.Should().BeEquivalentTo(expectedObject, options => options
                .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(100)))
                .WhenTypeIs<DateTime>()
            );
        }

        [Fact]
        public void AutoMapper_ConvertFrom_UserTimeout_To_NewBan_IsValid()
        {
            var mapper = Configuration.CreateMapper();

            var guid = Guid.NewGuid();

            var sourceObject = new UserTimeout("testChannel", "testUserName", 26, "testReason");

            var resultObject = mapper.Map<NewBan>(sourceObject);

            var expectedObject = new NewBan()
            {
                Channel = "testChannel",
                Reason = "testReason",
                BanType = Shared.Database.Models.BanType.Timeout,
                Duration = 26,
                BannedTime = DateTime.UtcNow,
                User = new User
                {
                    UserName = "testUserName",
                },
            };
            resultObject.Should().BeEquivalentTo(expectedObject, options => options
                .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(100)))
                .WhenTypeIs<DateTime>()
            );
        }
    }
}