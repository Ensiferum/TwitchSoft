namespace TwitchSoft.Shared.Database.Models
{
    public enum UserType : byte
    {
        Viewer = 0,
        Moderator = 1,
        GlobalModerator = 2,
        Broadcaster = 3,
        Admin = 4,
        Staff = 5
    }
}
