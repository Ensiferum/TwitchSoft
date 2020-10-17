using System;

namespace TwitchSoft.Shared.Services.Models
{
    public record ChatMessageModelForDisplaying
    {
        public string UserName { get; set; }
        public string Message { get; set; }
        public DateTime PostedTime { get; set; }
        public string Channel { get; set; }
    }
}
