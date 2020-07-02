using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
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
using System.Linq;
using TwitchSoft.TwitchBot.ChatPlugins;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace TwitchSoft.TwitchBot
{
    public class TwitchBot
    {
        private readonly ILogger<TwitchBot> logger;
        private readonly IServiceProvider serviceProvider;
        private readonly IEnumerable<IChatPlugin> chatPlugins;

        private ISendEndpointProvider Bus => serviceProvider.GetService<ISendEndpointProvider>();

        private const string JoinChannelsCommand = "JoinChannelsCommand";

        private BotSettings BotSettings { get; set; }
        //private static int LogMessagesCount { get; set; } = 0;
        //private static int LowMessagesCount { get; set; } = 0;

        private TwitchClient twitchClient;
        private HubConnection connection;
        //private Timer timer;

        public TwitchBot(
            ILogger<TwitchBot> logger, 
            IOptions<BotSettings> options,
            IServiceProvider serviceProvider,
            IEnumerable<IChatPlugin> chatPlugins)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.chatPlugins = chatPlugins;
            BotSettings = options.Value;
        }

        public void Start()
        {
            //timer = new Timer(CheckConnection, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
            Connect();
        }

        public void Stop()
        {
            //timer.Dispose();
            twitchClient.Disconnect();
        }

        public void JoinChannel(string channel)
        {
            logger.LogInformation($"JoinChannel {channel} triggered");
            twitchClient.JoinChannel(channel);
        }

        private void Connect()
        {
            try
            {
                InitTwitchBotClient();
                twitchClient.Connect();
                InitSignalRClient();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Connecting error");
            }
        }

        //private void CheckConnection(object state)
        //{
        //    logger.LogTrace($"Checking connection, MessagesCount: {LogMessagesCount}");
        //    logger.LogTrace($"Joined channels:{twitchClient.JoinedChannels.Count}");
        //    if (LogMessagesCount < 5)
        //    {
        //        LowMessagesCount++;
        //        if (LowMessagesCount >= 3)
        //        {
        //            logger.LogWarning($"{LogMessagesCount} log messages, trying to reconnect");
        //            try
        //            {
        //                twitchClient.Disconnect();
        //            }
        //            catch (Exception e)
        //            {
        //                logger.LogError(e, "Failed to disconnect");
        //            }

        //            Connect();
        //        }
        //    }
        //    else
        //    {
        //        LowMessagesCount = 0;
        //    }
        //    LogMessagesCount = 0;
        //}

        private void InitSignalRClient()
        {
            connection = new HubConnectionBuilder()
                .WithUrl("https://ts-twitchbotorchestrator/orchestration")
                .ConfigureLogging(_ => _.SetMinimumLevel(LogLevel.Trace))
                .AddJsonProtocol()
                .WithAutomaticReconnect()
                .Build();

            connection.On<IEnumerable<string>>(JoinChannelsCommand, channels => RefreshJoinedChannels(channels));

            connection.Reconnected += Connection_Reconnected;

            connection.Closed += Connection_Closed;

            connection.StartAsync().ContinueWith(t => {
                if (t.IsFaulted)
                    logger.LogError(t.Exception.GetBaseException(), "Error hub connection");
                else
                    logger.LogInformation("Connected to Hub");

            }).Wait();

            logger.LogInformation($"HubInfo: {connection.ConnectionId}, {connection.State}");
        }

        private Task Connection_Closed(Exception arg)
        {
            logger.LogError(arg, "Hub Connection Closed");
            return Task.CompletedTask;
        }

        private Task Connection_Reconnected(string arg)
        {
            logger.LogInformation("Hub Connection Reconnected", arg);
            return Task.CompletedTask;
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
            //twitchClient.OnLog += Client_OnLog;

            twitchClient.OnNewSubscriber += Client_OnNewSubscriber;
            twitchClient.OnReSubscriber += Client_OnReSubscriber;
            twitchClient.OnGiftedSubscription += Client_OnGiftedSubscription;
            twitchClient.OnCommunitySubscription += Client_OnCommunitySubscription;

            // twitchClient.OnUserBanned += Client_OnUserBanned;
            // twitchClient.OnUserTimedout += Client_OnUserTimedout;
        }

        private void Client_OnLeftChannel(object sender, OnLeftChannelArgs e)
        {
            logger.LogInformation($"OnLeftChannel: {e.Channel}");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            logger.LogTrace($"OnJoinedChannel: {e.Channel}");
        }

        //private void Client_OnLog(object sender, OnLogArgs e)
        //{
        //    Action<string> action = (string data) => logger.LogTrace(data);
        //    if (LowMessagesCount >= 1)
        //    {
        //        action = (string data) => logger.LogWarning(data);
        //    }
        //    action($"OnLog: {e.Data}");
        //    LogMessagesCount++;
        //}

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

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            logger.LogInformation($"Connected to {e.AutoJoinChannel}");

            //var channels = await repository.GetChannelsToTrack();

            //foreach (var channel in channels)
            //{
            //    twitchClient.JoinChannel(channel.Username);
            //}
        }

        private async void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            var chatMessage = new NewTwitchChannelMessage()
            {
                Id = Guid.Parse(e.ChatMessage.Id),
                Channel = e.ChatMessage.Channel,
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
            await Bus.Send(chatMessage);

            foreach (var plugin in chatPlugins)
            {
                await plugin.ProcessMessage(e.ChatMessage, twitchClient);
            }
        }

        private async void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            var subInfo = e.Subscriber;
            var newSub = new NewSubscriber
            {
                Channel = e.Channel,
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

            await Bus.Send(newSub);
        }


        private async void Client_OnReSubscriber(object sender, OnReSubscriberArgs e)
        {
            var subInfo = e.ReSubscriber;
            var newSub = new NewSubscriber
            {
                Channel = e.Channel,
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

            await Bus.Send(newSub);
        }

        private async void Client_OnGiftedSubscription(object sender, OnGiftedSubscriptionArgs e)
        {
            var subInfo = e.GiftedSubscription;
            var newSub = new NewSubscriber
            {
                Channel = e.Channel,
                Id = Guid.Parse(subInfo.Id),
                SubscribedTime = DateTimeHelper.FromUnixTimeToUTC(subInfo.TmiSentTs),
                Months = subInfo.MsgParamMonths != null ? int.Parse(subInfo.MsgParamMonths) : 0,
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

            await Bus.Send(newSub);
        }

        private async void Client_OnCommunitySubscription(object sender, OnCommunitySubscriptionArgs e)
        {
            var comSubInfo = e.GiftedSubscription;
            var newSub = new NewCommunitySubscription
            {
                Channel = e.Channel,
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

            await Bus.Send(newSub);
        }

        private async void Client_OnUserBanned(object sender, OnUserBannedArgs e)
        {
            var banInfo = e.UserBan;
            var newBan = new NewBan
            {
                Channel = banInfo.Channel,
                Reason = banInfo.BanReason,
                BannedTime = DateTime.UtcNow,
                BanType = BanType.Ban,
                User = new User
                {
                    // we have no userId here
                    UserName = banInfo.Username,
                },
            };

            await Bus.Send(newBan);
        }


        private async void Client_OnUserTimedout(object sender, OnUserTimedoutArgs e)
        {
            var banInfo = e.UserTimeout;
            var newBan = new NewBan
            {
                Channel = banInfo.Channel,
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

            await Bus.Send(newBan);
        }

        public void RefreshJoinedChannels(IEnumerable<string> channels)
        {
            var joinedChannels = twitchClient.JoinedChannels;

            foreach (var channel in joinedChannels)
            {
                if (channels.Any(_ => _.Equals(channel.Channel, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }
                else
                {
                    twitchClient.LeaveChannel(channel.Channel);
                }
            }

            foreach (var channel in channels)
            {
                if (joinedChannels.Any(_ => _.Channel.Equals(channel, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }
                else
                {
                    twitchClient.JoinChannel(channel);
                }
            }
        }
    }
}
