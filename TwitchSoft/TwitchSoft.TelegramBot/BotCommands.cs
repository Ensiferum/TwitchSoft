namespace TwitchSoft.TelegramBot
{
    //usermessages - Сообщения для юзера
    //addchannel - Добавить канал для отслеживания
    //subscriberscount - Показать кол-во сабов для канала
    //topbysubscribers - Показать топ по кол-ву сабов
    //searchtext - Поиск по тексту сообщений
    //dailynewsubs - Кол-во новых сабов за 24 часа

    public static class BotCommands
    {
        public const string AddChannel = "/addchannel";
        public const string SearchText = "/searchtext";
        public const string UserMessages = "/usermessages";
        public const string SubscribersCount = "/subscriberscount";
        public const string TopBySubscribers = "/topbysubscribers";
        public const string NewSubscribersCountDaily = "/dailynewsubs";
    }

    public enum BotCommand
    {
        AddChannel,
        SearchText,
        UserMessages,
        SubscribersCount,
        TopBySubscribers,
        NewSubscribersCountDaily
    }
}