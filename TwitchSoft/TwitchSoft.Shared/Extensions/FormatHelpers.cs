using System;
using System.Web;
using TwitchSoft.Shared.Services.Models;

namespace TwitchSoft.Shared.Extensions
{
    public static class FormatHelpers
    {
        public static string ToDisplayFormat(this ChatMessageModelForDisplaying message)
        {
            var postedTime = message.PostedTime.ConvertToMyTimezone();
            var dateFormat = postedTime.Year == postedTime.Year ? "dd/MM HH:mm:ss" : "dd/MM/yyyy HH:mm:ss";
            if (postedTime.Date == DateTime.Today)
            {
                dateFormat = "HH:mm:ss";
            }
            return $@"
{postedTime.ToString(dateFormat)}: Channel: <b>{message.Channel}</b>
<b>{message.UserName}</b>: {HttpUtility.HtmlEncode(message.Message)}";
        }
    }
}
