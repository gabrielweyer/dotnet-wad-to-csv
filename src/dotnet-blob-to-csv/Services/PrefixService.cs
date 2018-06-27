using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Blob;
using static System.String;

namespace DotNet.BlobToCsv.Services
{
    public static class PrefixService
    {
        private const string MonthFormat = "yyyy/MM";
        private static readonly string DayFormat = $"{MonthFormat}/dd";
        private static readonly string HourFormat = $"{DayFormat}/HH";

        public static List<string> BuildBlobPrefixes(DateTime from, DateTime to, string prefix)
        {
            return BuildBlobPrefixes(from, to).Select(p => $"{prefix}{p}").ToList();
        }

        private static List<string> BuildBlobPrefixes(DateTime from, DateTime to)
        {
            var difference = to - from;

            if (difference < TimeSpan.FromHours(1))
            {
                var prefixes = new List<string>
                {
                    GetHourPrefix(from)
                };

                if (from.Hour != to.Hour)
                {
                    prefixes.Add(GetHourPrefix(to));
                }

                return prefixes;
            }

            if (difference < TimeSpan.FromDays(1))
            {
                var prefixes = new List<string>
                {
                    GetDayPrefix(from)
                };

                if (from.Day != to.Day)
                {
                    prefixes.Add(GetDayPrefix(to));
                }

                return prefixes;
            }

            if (difference < TimeSpan.FromDays(28))
            {
                var prefixes = new List<string>
                {
                    GetMonthPrefix(from)
                };

                if (from.Month != to.Month)
                {
                    prefixes.Add(GetMonthPrefix(to));
                }

                return prefixes;
            }

            return Enumerable.Range(from.Year, to.Year - from.Year + 1).Select(y => y.ToString()).ToList();

            string GetDayPrefix(DateTime date)
            {
                return date.ToString(DayFormat);
            }

            string GetMonthPrefix(DateTime date)
            {
                return date.ToString(MonthFormat);
            }
        }

        public static List<CloudBlockBlob> Filter(List<CloudBlockBlob> blobs, DateTime from, DateTime to, string prefix)
        {
            return blobs.Where(b =>
                Compare(b.Name, $"{prefix}{GetHourPrefix(from)}", StringComparison.Ordinal) >= 0 &&
                Compare(b.Name, $"{prefix}{GetHourPrefix(to.AddHours(1))}", StringComparison.Ordinal) < 0).ToList();
        }

        private static string GetHourPrefix(DateTime date)
        {
            return $"{date.ToString(HourFormat)}";
        }
    }
}
