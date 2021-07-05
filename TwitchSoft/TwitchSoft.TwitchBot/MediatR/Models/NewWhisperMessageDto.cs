using MediatR;
using TwitchLib.Client.Models;

namespace TwitchSoft.TwitchBot.MediatR.Models
{
    public record NewWhisperMessageDto : IRequest
    {
        public WhisperMessage WhisperMessage { get; init; }
    }
}
