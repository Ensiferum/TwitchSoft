﻿using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Models;
using TwitchSoft.Shared.Database.Models;
using TwitchSoft.Shared.Services.Helpers;
using User = TwitchSoft.Shared.ServiceBus.Models.User;
using TwitchSoft.Shared.Services.Models.Twitch;
using TwitchSoft.Shared.ServiceBus.Models;
using System.Threading.Tasks;
using System.Linq;
using TwitchSoft.TwitchBot.ChatPlugins;
using System.Collections.Generic;

namespace TwitchSoft.TwitchBot
{
    public class TwitchBot
    {
        private readonly ILogger<TwitchBot> logger;
        private readonly ISendEndpointProvider bus;
        private readonly IChannelsCache channelsCache;
        private readonly IEnumerable<IChatPlugin> chatPlugins;

        private BotSettings BotSettings { get; set; }
        private static int LogMessagesCount { get; set; } = 0;
        private static int LowMessagesCount { get; set; } = 0;

        private TwitchClient twitchClient;
        private Timer timer;

        public TwitchBot(
            ILogger<TwitchBot> logger, 
            IOptions<BotSettings> options,
            ISendEndpointProvider bus,
            IChannelsCache channelsService,
            IEnumerable<IChatPlugin> chatPlugins)
        {
            this.logger = logger;
            this.bus = bus;
            this.channelsCache = channelsService;
            this.chatPlugins = chatPlugins;
            BotSettings = options.Value;

        }

        public void Start()
        {
            timer = new Timer(CheckConnection, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
            Connect();
        }

        public void Stop()
        {
            timer.Dispose();
            twitchClient.Disconnect();
        }

        public void JoinChannel(string channel)
        {
            logger.LogInformation($"JoinChannel {channel} triggered");
            channelsCache.InvalidateCache();
            twitchClient.JoinChannel(channel);
        }

        private void Connect()
        {
            try
            {
                InitTwitchBotClient();
                twitchClient.Connect();

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Connecting error");
            }
        }

        private void CheckConnection(object state)
        {
            logger.LogTrace($"Checking connection, MessagesCount: {LogMessagesCount}");
            logger.LogTrace($"Joined channels:{twitchClient.JoinedChannels.Count}");
            if (LogMessagesCount < 5)
            {
                LowMessagesCount++;
                if (LowMessagesCount >= 3)
                {
                    logger.LogWarning($"{LogMessagesCount} log messages, trying to reconnect");
                    try
                    {
                        twitchClient.Disconnect();
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, "Failed to disconnect");
                    }

                    Connect();
                }
            }
            else
            {
                LowMessagesCount = 0;
            }
            LogMessagesCount = 0;
        }

        private void InitTwitchBotClient()
        {
            ConnectionCredentials credentials = new ConnectionCredentials(BotSettings.BotName, BotSettings.BotOAuthToken);
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 5000,
                ThrottlingPeriod = TimeSpan.FromSeconds(1)
            };
            var customClient = new WebSocketClient(clientOptions);
            twitchClient = new TwitchClient(customClient);
            twitchClient.Initialize(credentials, autoReListenOnExceptions: false);

            twitchClient.OnDisconnected += Client_OnDisconnected;
            twitchClient.OnConnected += Client_OnConnected;
            twitchClient.OnMessageReceived += Client_OnMessageReceived;
            twitchClient.OnJoinedChannel += Client_OnJoinedChannel;
            twitchClient.OnLeftChannel += Client_OnLeftChannel;

            twitchClient.OnMessageThrottled += Client_OnMessageThrottled;
            twitchClient.OnConnectionError += Client_OnConnectionError;
            twitchClient.OnError += Client_OnError;
            twitchClient.OnReconnected += Client_OnReconnected;
            twitchClient.OnLog += Client_OnLog;

            twitchClient.OnNewSubscriber += Client_OnNewSubscriber;
            twitchClient.OnReSubscriber += Client_OnReSubscriber;
            twitchClient.OnGiftedSubscription += Client_OnGiftedSubscription;
            twitchClient.OnCommunitySubscription += Client_OnCommunitySubscription;

            twitchClient.OnUserBanned += Client_OnUserBanned;
            twitchClient.OnUserTimedout += Client_OnUserTimedout;

            
        }

        private void Client_OnLeftChannel(object sender, OnLeftChannelArgs e)
        {
            logger.LogInformation($"OnLeftChannel: {e.Channel}");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            logger.LogTrace($"OnJoinedChannel: {e.Channel}");
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            Action<string> action = (string data) => logger.LogTrace(data);
            if (LowMessagesCount >= 1)
            {
                action = (string data) => logger.LogWarning(data);
            }
            action($"OnLog: {e.Data}");
            LogMessagesCount++;
        }

        private void Client_OnUnaccountedFor(object sender, OnUnaccountedForArgs e)
        {
            logger.LogWarning($"OnUnaccountedFor: {e.BotUsername} {e.Channel} {e.Location} {e.RawIRC}");
        }

        private void Client_OnReconnected(object sender, OnReconnectedEventArgs e)
        {
            logger.LogWarning("OnReconnected");
        }

        private void Client_OnError(object sender, OnErrorEventArgs e)
        {
            logger.LogError(e.Exception, "Error");
        }

        private void Client_OnConnectionError(object sender, OnConnectionErrorArgs e)
        {
            logger.LogError($"ConnectionError\r\nMessage:{e.Error.Message}");
        }

        private void Client_OnMessageThrottled(object sender, OnMessageThrottledEventArgs e)
        {
            logger.LogWarning($"MessageThrottled\r\nMessage:{e.Message}\tSentMessageCount:{e.SentMessageCount}");
        }

        private void Client_OnDisconnected(object sender, OnDisconnectedEventArgs e)
        {
            logger.LogInformation($"Bot is disconnected");
        }

        private async void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            logger.LogInformation($"Connected to {e.AutoJoinChannel}");

            var channels = await channelsCache.GetTrackedChannels();

            foreach (var channel in channels)
            {
                twitchClient.JoinChannel(channel.Username);
            }
        }

        private async void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            var channel = await channelsCache.GetChannelByName(e.ChatMessage.Channel);
            if (channel.TrackMessages == true)
            {
                var chatMessage = new NewTwitchChannelMessage()
                {
                    Id = Guid.Parse(e.ChatMessage.Id),
                    Channel = User.FromDbUser(channel),
                    Message = e.ChatMessage.Message,
                    User = new User
                    {
                        UserId = uint.Parse(e.ChatMessage.UserId),
                        UserName = e.ChatMessage.Username,
                    },
                    IsBroadcaster = e.ChatMessage.IsBroadcaster,
                    IsModerator = e.ChatMessage.IsModerator,
                    IsSubscriber = e.ChatMessage.IsSubscriber,
                    PostedTime = DateTime.UtcNow,
                    UserType = Enum.Parse<UserType>(e.ChatMessage.UserType.ToString())
                };
                await bus.Send(chatMessage);
            }

            foreach (var plugin in chatPlugins)
            {
                plugin.ProcessMessage(e.ChatMessage, twitchClient);
            }
        }

        private async void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            var subInfo = e.Subscriber;
            var newSub = new NewSubscriber
            {
                Channel = User.FromDbUser(await channelsCache.GetChannelByName(e.Channel)),
                Id = Guid.Parse(subInfo.Id),
                SubscribedTime = DateTimeHelper.FromUnixTimeToUTC(subInfo.TmiSentTs),
                Months = 0,
                SubscriptionPlan = Enum.Parse<SubscriptionPlan>(subInfo.SubscriptionPlan.ToString()),
                User = new User
                {
                    UserId = uint.Parse(subInfo.UserId),
                    UserName = subInfo.Login,
                },
                UserType = Enum.Parse<UserType>(subInfo.UserType.ToString())
            };

            await bus.Send(newSub);
        }


        private async void Client_OnReSubscriber(object sender, OnReSubscriberArgs e)
        {
            var subInfo = e.ReSubscriber;
            var newSub = new NewSubscriber
            {
                Channel = User.FromDbUser(await channelsCache.GetChannelByName(e.Channel)),
                Id = Guid.Parse(subInfo.Id),
                SubscribedTime = DateTimeHelper.FromUnixTimeToUTC(subInfo.TmiSentTs),
                Months = int.Parse(subInfo.MsgParamCumulativeMonths),
                SubscriptionPlan = Enum.Parse<SubscriptionPlan>(subInfo.SubscriptionPlan.ToString()),
                User = new User
                {
                    UserId = uint.Parse(subInfo.UserId),
                    UserName = subInfo.Login,
                },
                UserType = Enum.Parse<UserType>(subInfo.UserType.ToString())
            };

            await bus.Send(newSub);
        }

        private async void Client_OnGiftedSubscription(object sender, OnGiftedSubscriptionArgs e)
        {
            var subInfo = e.GiftedSubscription;
            var newSub = new NewSubscriber
            {
                Channel = User.FromDbUser(await channelsCache.GetChannelByName(e.Channel)),
                Id = Guid.Parse(subInfo.Id),
                SubscribedTime = DateTimeHelper.FromUnixTimeToUTC(subInfo.TmiSentTs),
                Months = subInfo.MsgParamCumulativeMonths != null ? int.Parse(subInfo.MsgParamCumulativeMonths) : 0,
                SubscriptionPlan = (SubscriptionPlan)subInfo.MsgParamSubPlan,
                User = new User
                {
                    UserId = uint.Parse(subInfo.MsgParamRecipientId),
                    UserName = subInfo.MsgParamRecipientUserName,
                },
                UserType = Enum.Parse<UserType>(subInfo.UserType.ToString()),
                GiftedBy = new User {
                    UserId = uint.Parse(subInfo.UserId),
                    UserName = subInfo.Login,
                }
            };

            await bus.Send(newSub);
        }

        private async void Client_OnCommunitySubscription(object sender, OnCommunitySubscriptionArgs e)
        {
            var comSubInfo = e.GiftedSubscription;
            var newSub = new NewCommunitySubscription
            {
                Channel = User.FromDbUser(await channelsCache.GetChannelByName(e.Channel)),
                Id = Guid.Parse(comSubInfo.Id),
                Date = DateTimeHelper.FromUnixTimeToUTC(comSubInfo.TmiSentTs),
                SubscriptionPlan = (SubscriptionPlan)comSubInfo.MsgParamSubPlan,
                User = new User
                {
                    UserId = uint.Parse(comSubInfo.UserId),
                    UserName = comSubInfo.Login,
                },
                GiftCount = comSubInfo.MsgParamMassGiftCount,
            };

            await bus.Send(newSub);
        }

        private async void Client_OnUserBanned(object sender, OnUserBannedArgs e)
        {
            var channel = await channelsCache.GetChannelByName(e.UserBan.Channel);
            if (channel.TrackMessages == true)
            {
                var banInfo = e.UserBan;
                var newBan = new NewBan
                {
                    Channel = User.FromDbUser(await channelsCache.GetChannelByName(banInfo.Channel)),
                    Reason = banInfo.BanReason,
                    BannedTime = DateTime.UtcNow,
                    BanType = BanType.Ban,
                    User = new User
                    {
                        // we have no userId here
                        UserName = banInfo.Username,
                    },
                };

                await bus.Send(newBan);
            }
        }


        private async void Client_OnUserTimedout(object sender, OnUserTimedoutArgs e)
        {
            var channel = await channelsCache.GetChannelByName(e.UserTimeout.Channel);
            if (channel.TrackMessages == true)
            {
                var banInfo = e.UserTimeout;
                var newBan = new NewBan
                {
                    Channel = User.FromDbUser(await channelsCache.GetChannelByName(banInfo.Channel)),
                    Reason = banInfo.TimeoutReason,
                    BannedTime = DateTime.UtcNow,
                    BanType = BanType.Timeout,
                    Duration = banInfo.TimeoutDuration,
                    User = new User
                    {
                        // we have no userId here
                        UserName = banInfo.Username,
                    },
                };

                await bus.Send(newBan);
            }
        }

        public async Task RefreshJoinedChannels()
        {
            channelsCache.InvalidateCache();
            var channels = await channelsCache.GetTrackedChannels();
            var joinedChannels = twitchClient.JoinedChannels;
            foreach (var chan in channels)
            {
                if (joinedChannels.Any(_ => _.Channel.Equals(chan.Username, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }
                else
                {
                    twitchClient.JoinChannel(chan.Username);
                }
            }
        }
    }
}