using AutoMapper;
using System;
using TwitchLib.Client.Models;
using TwitchSoft.Shared.Database.Models;
using TwitchSoft.Shared.ServiceBus.Models;
using TwitchSoft.Shared.Services.Helpers;
using TwitchSoft.TwitchBot.MediatR.Models;
using User = TwitchSoft.Shared.ServiceBus.Models.User;

namespace TwitchSoft.TwitchBot
{
    public class TwitchBotProfile : Profile
    {
        public TwitchBotProfile()
        {
            CreateMap<ChatMessage, NewTwitchChannelMessage>()
                .ForMember(dest => dest.PostedTime, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => Enum.Parse<UserType>(src.UserType.ToString())))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => new User {
                    UserId = uint.Parse(src.UserId),
                    UserName = src.Username,
                }));

            CreateMap<SubscriberBase, NewSubscriber>()
                .ForMember(dest => dest.SubscribedTime, opt => opt.MapFrom(src => DateTimeHelper.FromUnixTimeToUTC(src.TmiSentTs)))
                .ForMember(dest => dest.SubscriptionPlan, opt => opt.MapFrom(src => Enum.Parse<SubscriptionPlan>(src.SubscriptionPlan.ToString())))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => new User
                {
                    UserId = uint.Parse(src.UserId),
                    UserName = src.Login,
                }))
                .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => Enum.Parse<UserType>(src.UserType.ToString())))
                .ForMember(dest => dest.Months, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.GiftedBy, opt => opt.Ignore())
                .IncludeAllDerived();

            CreateMap<Subscriber, NewSubscriber>();

            CreateMap<NewSubscriberDto, NewSubscriber>()
                .IncludeMembers(src => src.Subscriber)
                .ForMember(dest => dest.GiftedBy, opt => opt.Ignore());

            CreateMap<ReSubscriber, NewSubscriber>()
                .ForMember(dest => dest.Months, opt => opt.MapFrom(src => int.Parse(src.MsgParamCumulativeMonths)))
                .ForMember(dest => dest.GiftedBy, opt => opt.Ignore());

            CreateMap<NewResubscriberDto, NewSubscriber>()
                .IncludeMembers(src => src.ReSubscriber)
                .ForMember(dest => dest.GiftedBy, opt => opt.Ignore());

            CreateMap<GiftedSubscription, NewSubscriber>()
                .ForMember(dest => dest.Channel, opt => opt.Ignore())
                .ForMember(dest => dest.SubscribedTime, opt => opt.MapFrom(src => DateTimeHelper.FromUnixTimeToUTC(src.TmiSentTs)))
                .ForMember(dest => dest.SubscriptionPlan, opt => opt.MapFrom(src => (SubscriptionPlan)src.MsgParamSubPlan))
                .ForMember(dest => dest.Months, opt => opt.MapFrom(src =>
                    src.MsgParamMonths != null ? int.Parse(src.MsgParamMonths) : 0))
                .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => Enum.Parse<UserType>(src.UserType.ToString())))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => new User
                {
                    UserId = uint.Parse(src.MsgParamRecipientId),
                    UserName = src.MsgParamRecipientUserName,
                }))
                .ForMember(dest => dest.GiftedBy, opt => opt.MapFrom(src => new User
                {
                    UserId = uint.Parse(src.UserId),
                    UserName = src.Login,
                }));

            CreateMap<NewGiftedSubscriptionDto, NewSubscriber>()
                .IncludeMembers(src => src.GiftedSubscription);

            CreateMap<TwitchLib.Client.Models.CommunitySubscription, NewCommunitySubscription>()
                .ForMember(dest => dest.Channel, opt => opt.Ignore())
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => DateTimeHelper.FromUnixTimeToUTC(src.TmiSentTs)))
                .ForMember(dest => dest.SubscriptionPlan, opt => opt.MapFrom(src => (SubscriptionPlan)src.MsgParamSubPlan))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => new User
                {
                    UserId = uint.Parse(src.UserId),
                    UserName = src.Login,
                }))
                .ForMember(dest => dest.GiftCount, opt => opt.MapFrom(src => src.MsgParamMassGiftCount));

            CreateMap<NewCommunitySubscriptionDto, NewCommunitySubscription>()
                .IncludeMembers(src => src.CommunitySubscription);

            CreateMap<TwitchLib.Client.Models.UserBan, NewBan>()
                .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.BanReason))
                .ForMember(dest => dest.BannedTime, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.BanType, opt => opt.MapFrom(src => BanType.Ban))
                .ForMember(dest => dest.Duration, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => new User
                {
                    UserId = uint.Parse(src.TargetUserId),
                    UserName = src.Username,
                }));

            CreateMap<UserTimeout, NewBan>()
                .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.TimeoutReason))
                .ForMember(dest => dest.BannedTime, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.BanType, opt => opt.MapFrom(src => BanType.Timeout))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.TimeoutDuration))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => new User
                {
                    UserName = src.Username,
                }));
        }
    }
}
