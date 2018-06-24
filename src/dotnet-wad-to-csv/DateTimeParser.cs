using System;
using System.Globalization;

namespace DotNet.WadToCsv
{
    public static class DateTimeParser
    {
        public static bool TryParseIso8601DateTime(this string input, out DateTime output)
        {
            return DateTime.TryParseExact(
                input,
                new[] { "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH", "yyyy-MM-dd" },
                DateTimeFormatInfo.InvariantInfo,
                DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
                out output);
        }
    }
}
