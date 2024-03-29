﻿using Microsoft.Extensions.Logging;
using System;
using TwitchLib.Client.Events;
using TwitchLib.Communication.Events;
using System.Linq;
using TwitchLib.Client.Interfaces;
using TwitchSoft.TwitchBot.MediatR.Models;
using MediatR;
using System.Collections.Generic;
using System.Threading.Tasks;
using Polly;
using System.Text.RegularExpressions;

namespace TwitchSoft.TwitchBot
{
    public class TwitchBot
    {
        private readonly ILogger<TwitchBot> logger;
        private readonly ITwitchClient twitchClient;
        private readonly IMediator mediator;

        private int EventsCount;

        private readonly List<string> JoinedChannels = new();

        public TwitchBot(
            ILogger<TwitchBot> logger, 
            ITwitchClient twitchClient,
            IMediator mediator)
        {
            this.logger = logger;
            this.twitchClient = twitchClient;
            this.mediator = mediator;
        }

        public void Start()
        {
            Connect();
        }

        public void Stop()
        {
            twitchClient.Disconnect();
        }

        private void Connect()
        {
            try
            {
                InitTwitchBotEvents();
                twitchClient.Connect();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Connecting error");
            }
        }

        private void InitTwitchBotEvents()
        {
            twitchClient.OnDisconnected += Client_OnDisconnected;
            twitchClient.OnConnected += Client_OnConnected;
            twitchClient.OnMessageReceived += Client_OnMessageReceived;
            twitchClient.OnJoinedChannel += Client_OnJoinedChannel;
            twitchClient.OnLeftChannel += Client_OnLeftChannel;

            twitchClient.OnWhisperReceived += Client_OnWhisperReceived;

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

        private async void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            logger.LogTrace($"OnWhisperReceived: {e.WhisperMessage.Username} - {e.WhisperMessage.Message}");
            await mediator.Send(new NewWhisperMessageDto
            {
                WhisperMessage = e.WhisperMessage,
            });
        }

        private void Client_OnLeftChannel(object sender, OnLeftChannelArgs e)
        {
            logger.LogTrace($"OnLeftChannel: {e.Channel}");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            logger.LogTrace($"OnJoinedChannel: {e.Channel}");
        }

        private async void Client_OnLog(object sender, OnLogArgs e)
        {
            EventsCount++;
            logger.LogTrace($"OnLog:\r\nDate: {e.DateTime}\r\nData: {e.Data}");

            if (e.Data.StartsWith("Received: @msg-id=msg_channel_suspended"))
            {
                var regex = Regex.Match(e.Data, @"^Received: @msg-id=msg_channel_suspended :tmi.twitch.tv NOTICE #(?<channel>\w*) .*$");
                var channelName = regex.Groups["channel"].Value;

                logger.LogWarning($"Channel was suspended: {channelName}");

                await mediator.Send(new SetChannelBanned
                {
                    Channel = channelName,
                    IsBanned = true,
                });

                JoinedChannels.Remove(channelName);
            }

            if (e.Data.StartsWith("Received: @msg-id=msg_banned"))
            {
                var regex = Regex.Match(e.Data, @"^Received: @msg-id=msg_banned :tmi.twitch.tv NOTICE #(?<channel>\w*) :You are permanently banned from talking in");
                var channelName = regex.Groups["channel"].Value;

                logger.LogWarning($"Bot was banned from channel: {channelName}");

                await mediator.Send(new SetChannelBanned
                {
                    Channel = channelName,
                    IsBanned = true,
                });

                JoinedChannels.Remove(channelName);
            }
        }

        private void Client_OnReconnected(object sender, OnReconnectedEventArgs e)
        {
            logger.LogInformation("OnReconnected", e);
        }

        private void Client_OnError(object sender, OnErrorEventArgs e)
        {
            logger.LogError(e.Exception, "Error");
        }

        private void Client_OnConnectionError(object sender, OnConnectionErrorArgs e)
        {
            logger.LogError($"ConnectionError\r\nMessage:{e.Error.Message}");
        }

        private void Client_OnDisconnected(object sender, OnDisconnectedEventArgs e)
        {
            logger.LogTrace($"Bot is disconnected", e);
            twitchClient.Connect();
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            logger.LogTrace($"Connected to {e.AutoJoinChannel}");
        }

        private async void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            await mediator.Send(new NewChatMessageDto
            {
                ChatMessage = e.ChatMessage,
            });
        }

        private async void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            await mediator.Send(new NewSubscriberDto
            {
                Subscriber = e.Subscriber,
                Channel = e.Channel,
            });
        }


        private async void Client_OnReSubscriber(object sender, OnReSubscriberArgs e)
        {
            await mediator.Send(new NewResubscriberDto
            {
                ReSubscriber = e.ReSubscriber,
                Channel = e.Channel,
            });
        }

        private async void Client_OnGiftedSubscription(object sender, OnGiftedSubscriptionArgs e)
        {
            await mediator.Send(new NewGiftedSubscriptionDto
            {
                GiftedSubscription = e.GiftedSubscription,
                Channel = e.Channel,
            });
        }

        private async void Client_OnCommunitySubscription(object sender, OnCommunitySubscriptionArgs e)
        {
            await mediator.Send(new NewCommunitySubscriptionDto
            {
                CommunitySubscription = e.GiftedSubscription,
                Channel = e.Channel,
            });
        }

        private async void Client_OnUserBanned(object sender, OnUserBannedArgs e)
        {
            await mediator.Send(new NewUserBanDto
            {
                UserBan = e.UserBan,
            });
        }


        private async void Client_OnUserTimedout(object sender, OnUserTimedoutArgs e)
        {
            await mediator.Send(new NewUserTimeoutDto
            {
                UserTimeout = e.UserTimeout,
            });
        }

        public Task TriggerChannelsJoin()
        {
            if (twitchClient.IsConnected)
            {
                var joinedChannels = twitchClient.JoinedChannels.Select(_ => _.Channel.ToLower()).ToList();
                var newChannels = JoinedChannels.ToList();

                var channelsToLeave = joinedChannels.Except(newChannels);
                var channelsToConnect = newChannels.Except(joinedChannels);


                var logChannels = channelsToConnect.Select(_ => $"+{_}").Union(channelsToLeave.Select(_ => $"-{_}"));

                if (!logChannels.Any())
                {
                    return Task.CompletedTask;
                }

                logger.LogInformation($"New Channels:\r\n{string.Join("\r\n", logChannels)}");

                foreach (var channel in channelsToLeave)
                {
                    twitchClient.LeaveChannel(channel);
                }

                foreach (var channel in channelsToConnect)
                {
                    twitchClient.JoinChannel(channel);
                }
            }
            else
            {
                twitchClient.Reconnect();
            }
            return Task.CompletedTask;
        }

        public void SetChannels(IEnumerable<string> channels)
        {
            logger.LogInformation($"SetChannels. Channels: {string.Join(", ", channels)}");
            JoinedChannels.Clear();
            JoinedChannels.AddRange(channels.Select(chan => chan.ToLower()));
            _ = TriggerChannelsJoin();
        }

        public async Task CheckIfStillConnected()
        {
            logger.LogTrace($"Log Events count: {EventsCount}");

            if (EventsCount == 0)
            {
                logger.LogInformation($"Trying to reconnect.");
                try
                {
                    twitchClient.Disconnect();
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Failed to disconnect");
                }

                var policy = Policy.Handle<Exception>()
                    .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan) =>
                    {
                        logger.LogError($"Policy logging. Wait: {timeSpan.TotalSeconds}\r\n{exception.Message}\r\n{exception.StackTrace}");
                    });

                policy.Execute(() => twitchClient.Connect());
                await TriggerChannelsJoin();
            }
            else
            {
                EventsCount = 0;
            }
        }
    }
}
