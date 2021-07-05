using AutoMapper;
using Telegram.Bot.Types;
using TwitchSoft.TelegramBot.MediatR.Models;

namespace TwitchSoft.TelegramBot
{
    public class TelegramBotProfile : Profile
    {
        public TelegramBotProfile()
        {
            CreateMap<DigestInfoRequest, UserMessagesDigestCommand>()
                .ForMember(dest => dest.TwitchUserId, opt => {
                    opt.Condition(src => src.TwitchUserId != 0);
                    opt.MapFrom(src => src.TwitchUserId);
                });

            CreateMap<SendMessageRequest, SendMessageCommand>();

            CreateMap<InlineQuery, InlineUsersSearchCommand>()
                .ForMember(dest => dest.SearchUserText, opt => opt.MapFrom(_ => _.Query))
                .ForMember(dest => dest.InlineQueryId, opt => opt.MapFrom(_ => _.Id));
        }
    }
}
