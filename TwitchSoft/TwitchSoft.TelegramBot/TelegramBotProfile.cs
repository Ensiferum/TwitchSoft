using AutoMapper;
using Telegram.Bot.Types;
using TwitchSoft.TelegramBot.MediatR.Models;

namespace TwitchSoft.TelegramBot
{
    public class TelegramBotProfile : Profile
    {
        public TelegramBotProfile()
        {
            CreateMap<DigestInfoRequest, UserMessagesDigest>();

            CreateMap<InlineQuery, InlineUsersSearch>()
                .ForMember(dest => dest.SearchUserText, opt => opt.MapFrom(_ => _.Query))
                .ForMember(dest => dest.InlineQueryId, opt => opt.MapFrom(_ => _.Id));
        }
    }
}
