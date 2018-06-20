using System;
using System.Text.RegularExpressions;

namespace DotNet.WadToCsv
{
    public static class DurationParser
    {
        private const string HoursGroupName = "hours";
        private const string MinutesGroupName = "minutes";
        private const string SecondsGroupName = "seconds";

        private static readonly Regex Matcher = new Regex(
            $"((?<{HoursGroupName}>\\d+)H)?((?<{MinutesGroupName}>\\d+)M)?((?<{SecondsGroupName}>\\d+)S)?",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant,
            TimeSpan.FromSeconds(5));

        public static TimeSpan ParseIso8601TimeDuration(this string duration)
        {
            if (string.IsNullOrEmpty(duration))
            {
                throw new FormatException("This is not a valid ISO 8601 time duration.");
            }

            var match = Matcher.Match(duration);

            if (string.IsNullOrEmpty(match.Value))
            {
                throw new FormatException("This is not a valid ISO 8601 time duration.");
            }

            var hoursDuration = GetHours();
            var minutesDuration = GetMinutes();
            var secondsDuration = GetSeconds();

            return hoursDuration + minutesDuration + secondsDuration;

            TimeSpan GetHours()
            {
                return GetTimeComponent(HoursGroupName, TimeSpan.FromHours);
            }

            TimeSpan GetMinutes()
            {
                return GetTimeComponent(MinutesGroupName, TimeSpan.FromMinutes);
            }

            TimeSpan GetSeconds()
            {
                return GetTimeComponent(SecondsGroupName, TimeSpan.FromSeconds);
            }

            TimeSpan GetTimeComponent(string groupName, Func<double, TimeSpan> func)
            {
                var group = match.Groups[groupName];

                return group.Success ? func(long.Parse(group.Value)) : TimeSpan.Zero;
            }
        }
    }
}
