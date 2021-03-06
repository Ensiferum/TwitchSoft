﻿using MediatR;

namespace TwitchSoft.TelegramBot.MediatR.Models
{
    public class InlineUsersSearchCommand : IRequest
    {
        public string InlineQueryId { get; init; }
        public string SearchUserText { get; init; }
    }
}
