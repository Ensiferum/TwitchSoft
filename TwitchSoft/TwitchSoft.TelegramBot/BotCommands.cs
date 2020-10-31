namespace TwitchSoft.TelegramBot
{
    //start - Начать использовать бота
    //usermessages - Сообщения для юзера
    //addchannel - Добавить канал для отслеживания
    //subscriberscount - Показать кол-во сабов для канала
    //topbysubscribers - Показать топ по кол-ву сабов
    //search - Поиск по тексту сообщений

    public static class BotCommands
    {
        public const string AddChannel = "/addchannel";
        public const string SearchText = "/search";
        public const string UserMessages = "/usermessages";
        public const string SubscribersCount = "/subscriberscount";
        public const string TopBySubscribers = "/topbysubscribers";
    }
}