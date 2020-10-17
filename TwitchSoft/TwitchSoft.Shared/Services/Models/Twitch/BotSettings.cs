namespace TwitchSoft.Shared.Services.Models.Twitch
{
    public record BotSettings
    {
        public string BotName { get; set; }
        public string BotOAuthToken { get; set; }
        public string ClientId { get; set; }
        public string AccessToken { get; set; }
    }
}
