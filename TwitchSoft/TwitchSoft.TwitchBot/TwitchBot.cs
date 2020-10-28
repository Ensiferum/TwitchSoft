using Microsoft.Extensions.Logging;
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

namespace TwitchSoft.TwitchBot
{
    public class TwitchBot
    {
        private readonly ILogger<TwitchBot> logger;
        private readonly ITwitchClient twitchClient;
        private readonly IMediator mediator;

        private int EventsCount;

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
            logger.LogInformation($"OnLog:\r\nDate: {e.DateTime}\r\nData: {e.Data}");
            EventsCount++;
        }

        private void Client_OnReconnected(object sender, OnReconnectedEventArgs e)
        {
            logger.LogWarning("OnReconnected", e);
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
            logger.LogInformation($"Bot is disconnected", e);
            twitchClient.Connect();
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            logger.LogInformation($"Connected to {e.AutoJoinChannel}");
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

        public async Task TriggerChannelsJoin()
        {
            const int DelayMs = 50;
            if (twitchClient.IsConnected)
            {
                var joinedChannels = twitchClient.JoinedChannels.Select(_ => _.Channel.ToLower()).ToList();
                var newChannels = JoinedChannels.Select(chan => chan.ToLower()).ToList();

                var channelsToLeave = joinedChannels.Except(newChannels);
                var channelsToConnect = newChannels.Except(joinedChannels);


                var logChannels = channelsToConnect.Select(_ => $"+{_}").Union(channelsToLeave.Select(_ => $"-{_}"));

                if (!logChannels.Any())
                {
                    return;
                }

                logger.LogInformation($"New Channels:\r\n{string.Join("\r\n", logChannels)}");

                foreach (var channel in channelsToLeave)
                {
                    twitchClient.LeaveChannel(channel);
                    await Task.Delay(DelayMs);
                }

                foreach (var channel in channelsToConnect)
                {
                    twitchClient.JoinChannel(channel);
                    await Task.Delay(DelayMs);
                }
            }
            else
            {
                twitchClient.Reconnect();
            }
        }

        public void SetChannels(IEnumerable<string> channels)
        {
            logger.LogInformation($"SetChannels. Channels: {string.Join(", ", channels)}");
            JoinedChannels.Clear();
            JoinedChannels.AddRange(channels);
        }

        public void CheckIfStillConnected()
        {
            logger.LogInformation($"Log Events count: {EventsCount}");

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
            }
            else
            {
                EventsCount = 0;
            }
        }
    }
}
