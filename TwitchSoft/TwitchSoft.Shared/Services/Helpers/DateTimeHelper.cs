using System;

namespace TwitchSoft.Shared.Services.Helpers
{
    public static class DateTimeHelper
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

        public static DateTime ConvertToMyTimezone(this DateTime date)
        {
            var timeZoneId = "Belarus Standard Time";
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(date, timeZone);
        }
    }
}
