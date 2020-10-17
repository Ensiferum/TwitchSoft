namespace TwitchSoft.Shared.ServiceBus.Models
{
    public record User
    {
        public uint UserId { get; set; }
        public string UserName { get; set; }

        public static User FromDbUser(Database.Models.User user)
        {
            if (user == null) return null;

            return new User
            {
                UserId = user.Id,
                UserName = user.Username
            };
        }
    }
}
