﻿using MediatR;
using TwitchLib.Client.Models;

namespace TwitchSoft.TwitchBot.MediatR.Models
{
    public record NewChatMessageDto: IRequest
    {
        public ChatMessage ChatMessage { get; init; }
    }
}
