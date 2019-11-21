using System;

namespace TwitchSoft.Shared.Services.Helpers
{
    public class DateTimeHelper
    {
        public static DateTime FromUnixTimeToUTC(long unixDateTime)
        {
            var dateTime = DateTimeOffset.FromUnixTimeMilliseconds(unixDateTime).DateTime;
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }

        public static DateTime FromUnixTimeToUTC(string unixDateTimeString)
        {
            return FromUnixTimeToUTC(long.Parse(unixDateTimeString));
        }
    }
}
