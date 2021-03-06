﻿using System;

namespace TwitchSoft.Shared.Extensions
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
            string timeZoneId;
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                timeZoneId = "Europe/Minsk";
            }
            else
            {
                timeZoneId = "Belarus Standard Time";
            }
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(date, timeZone);
        }
    }
}
