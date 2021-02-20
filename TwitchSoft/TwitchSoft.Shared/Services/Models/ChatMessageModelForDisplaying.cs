using System;

namespace TwitchSoft.Shared.Services.Models
{
    public record ChatMessageModelForDisplaying
    {
        public string UserName { get; init; }
        public string Message { get; init; }
        public DateTime PostedTime { get; init; }
        public string Channel { get; init; }
    }
}
