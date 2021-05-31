using System;
using System.Linq;

namespace TwitchSoft.Shared.Extensions
{
    public static class StringExtensions
    {
        public static bool ContainsAny(this string input, params string[] containsKeywords)
        {
            return containsKeywords.Any(keyword => input.Contains(keyword, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
