namespace TwitchSoft.Shared.Models
{
    public record SimpleUserModel
    {
        public long Id { get; init; }
        public string UserName { get; init; }
    }
}
