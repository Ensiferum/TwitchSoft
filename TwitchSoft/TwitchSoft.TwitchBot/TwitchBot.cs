using Microsoft.Extensions.Logging;
using System;
using TwitchLib.Client.Events;
using TwitchLib.Communication.Events;
using System.Linq;
using System.Threading;
using TwitchLib.Client.Interfaces;
using TwitchSoft.TwitchBot.MediatR.Models;
using MediatR;
using System.Threading.Tasks;
using System.Collections.Generic;
using Polly;

namespace TwitchSoft.TwitchBot
{
    public class TwitchBot
    {
        private readonly ILogger<TwitchBot> logger;
        private readonly ITwitchClient twitchClient;
        private readonly IMediator mediator;

        //private static int LogMessagesCount = 0;
        //private static int LowMessagesCount = 0;
        //private static int MessagesCountPer10Sec = 0;
        private readonly List<string> JoinedChannels = new List<string>();

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
            //timer.Dispose();
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

        //private void CheckConnection(object state)
        //{
        //    logger.LogInformation($"Messages per 10 seconds: {MessagesCountPer10Sec}");
        //    MessagesCountPer10Sec = 0;

        //    logger.LogTrace($"Checking connection, MessagesCount: {LogMessagesCount}");
        //    logger.LogTrace($"Joined channels count:{twitchClient.JoinedChannels.Count}");
            
        //    if (LogMessagesCount < 5)
        //    {
        //        LowMessagesCount++;
        //        if (LowMessagesCount >= 5)
        //        {
        //            logger.LogInformation($"Joined channels. Channels: {string.Join(", ", twitchClient.JoinedChannels.Select(_ => _.Channel))}");
        //            logger.LogWarning($"{LogMessagesCount} log messages, trying to reconnect");

        //            logger.LogInformation($"Connected channels: {string.Join(", ", twitchClient.JoinedChannels.Select(_ => _.Channel))}");
        //            var policy = Policy.Handle<Exception>()
        //                .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
        //                (exception, timeSpan) =>
        //            {
        //                logger.LogError($"Policy logging. Wait: {timeSpan.TotalSeconds}\r\n{exception.Message}\r\n{exception.StackTrace}");
        //            });

        //            policy.Execute(() => twitchClient.Reconnect());
        //        }
        //    }
        //    else
        //    {
        //        LowMessagesCount = 0;
        //    }
        //    //RefreshJoinedChannels(JoinedChannels);
        //    LogMessagesCount = 0;
        //}

        private void InitTwitchBotEvents()
        {
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
            logger.LogTrace($"OnLeftChannel: {e.Channel}");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            logger.LogTrace($"OnJoinedChannel: {e.Channel}");
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            logger.LogTrace(e.Data);
            //LogMessagesCount++;
        }

        private void Client_OnReconnected(object sender, OnReconnectedEventArgs e)
        {
            logger.LogWarning("OnReconnected", e);
            RefreshJoinedChannels(JoinedChannels);
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
            logger.LogInformation($"Bot is disconnected", e);
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            logger.LogInformation($"Connected to {e.AutoJoinChannel}");

            RefreshJoinedChannels(JoinedChannels);
        }

        private async void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            //MessagesCountPer10Sec++;
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

        public void RefreshJoinedChannels(IEnumerable<string> channels, bool withClear = false)
        {
            logger.LogInformation($"RefreshJoinedChannels triggered. Channels: {string.Join(", ", channels)}");
            if (withClear)
            {
                JoinedChannels.Clear();
                JoinedChannels.AddRange(channels);
            }

            if (twitchClient.IsConnected)
            {
                var joinedChannels = twitchClient.JoinedChannels.Select(_ => _.Channel.ToLower()).ToList();
                var newChannels = channels.Select(chan => chan.ToLower()).ToList();

                var channelsToLeave = joinedChannels.Except(newChannels);
                var channelsToConnect = newChannels.Except(joinedChannels);
                logger.LogInformation($"Channels To Connect\r\n{string.Join("\r\n", channelsToConnect.Select(_ => $"+{_}"))}");
                logger.LogInformation($"Channels To Leave\r\n{string.Join("\r\n", channelsToLeave.Select(_ => $"-{_}"))}");

                foreach (var channel in channelsToLeave)
                {
                    twitchClient.LeaveChannel(channel);
                }

                foreach (var channel in channelsToConnect)
                {
                    twitchClient.JoinChannel(channel);
                }
            }
        }
    }
}
