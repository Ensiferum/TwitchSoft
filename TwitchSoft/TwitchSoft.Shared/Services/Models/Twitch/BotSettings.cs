namespace TwitchSoft.Shared.Services.Models.Twitch
{
    public record BotSettings
    {
        public string BotName { get; init; }
        public string BotOAuthToken { get; init; }
        public string ClientId { get; init; }
        public string AccessToken { get; init; }
    }
}
