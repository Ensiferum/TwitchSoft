﻿using AutoMapper;
using FluentAssertions;
using Telegram.Bot.Types;
using TwitchSoft.TelegramBot;
using TwitchSoft.TelegramBot.MediatR.Models;
using Xunit;

namespace TwitchSoft.Tests
{
    public class TelegramBotProfileTests
    {
        private MapperConfiguration Configuration;

        public TelegramBotProfileTests()
        {
            this.Configuration = new MapperConfiguration(cfg => {
                cfg.AddProfile<TelegramBotProfile>();
            });
        }

        [Fact]
        public void AutoMapper_Configuration_IsValid()
        {
            Configuration.AssertConfigurationIsValid();
        }

        [Fact]
        public void AutoMapper_ConvertFrom_DigestInfoRequest_To_UserMessagesDigest_IsValid()
        {
            var mapper = Configuration.CreateMapper();


            var sourceObject = new DigestInfoRequest {
                ChatId = "testChatId",
                Username = "testUsername"
            };

            var resultObject = mapper.Map<UserMessagesDigest>(sourceObject);

            var expectedObject = new UserMessagesDigest()
            {
                ChatId = "testChatId",
                Username = "testUsername"
            };
            resultObject.Should().BeEquivalentTo(expectedObject);
        }

        [Fact]
        public void AutoMapper_ConvertFrom_InlineQuery_To_InlineUsersSearch_IsValid()
        {
            var mapper = Configuration.CreateMapper();


            var sourceObject = new InlineQuery
            {
                Id = "testId",
                Query = "testQuery"
            };

            var resultObject = mapper.Map<InlineUsersSearch>(sourceObject);

            var expectedObject = new InlineUsersSearch()
            {
                InlineQueryId = "testId",
                SearchUserText = "testQuery"
            };
            resultObject.Should().BeEquivalentTo(expectedObject);
        }
    }
}
