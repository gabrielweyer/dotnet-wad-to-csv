using System;
using System.Collections.Generic;
using DotNet.BlobToCsv.Services;
using FluentAssertions;
using Xunit;

namespace DotNet.WadToCsv.Tests.Services
{
    public class PrefixService_BuildBlobPrefixes_Tests
    {
        private const string Prefix = "Hi/";

        [Fact]
        public void GivenSameHour_ThenSingleResultAllTheWayToHours()
        {
            // Arrange

            var from = new DateTime(2018, 10, 6, 16, 7, 3, DateTimeKind.Utc);
            var to = from.AddMinutes(10);

            // Act

            var actual = PrefixService.BuildBlobPrefixes(from, to, Prefix);

            // Assert

            var expected = new List<string>
            {
                $"{Prefix}2018/10/06/16"
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GivenLessThan60MinutesDifferenceButDifferentYears_ThenLastHourOfFromYearAndFirstHourOfToYear()
        {
            // Arrange

            var from = new DateTime(2018, 12, 31, 23, 55, 15, DateTimeKind.Utc);
            var to = from.AddMinutes(10);

            // Act

            var actual = PrefixService.BuildBlobPrefixes(from, to, Prefix);

            // Assert

            var expected = new List<string>
            {
                $"{Prefix}2018/12/31/23",
                $"{Prefix}2019/01/01/00"
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Given60MinutesOrMoreDifferenceButLessThanDay_ThenAllTheWayToDay()
        {
            // Arrange

            var from = new DateTime(2018, 10, 6, 16, 7, 3, DateTimeKind.Utc);
            var to = from.AddMinutes(65);

            // Act

            var actual = PrefixService.BuildBlobPrefixes(from, to, Prefix);

            // Assert

            var expected = new List<string>
            {
                $"{Prefix}2018/10/06"
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Given60MinutesOrMoreDifferenceButLessThanDayAndDifferentYears_ThenLastDayOfFromYearAndFirstDayOfToYear()
        {
            // Arrange

            var from = new DateTime(2018, 12, 31, 23, 55, 15, DateTimeKind.Utc);
            var to = from.AddMinutes(65);

            // Act

            var actual = PrefixService.BuildBlobPrefixes(from, to, Prefix);

            // Assert

            var expected = new List<string>
            {
                $"{Prefix}2018/12/31",
                $"{Prefix}2019/01/01"
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GivenDayOrMoreDifferenceButLessThan28Days_ThenAllTheWayToMonth()
        {
            // Arrange

            var from = new DateTime(2018, 10, 6, 16, 7, 3, DateTimeKind.Utc);
            var to = from.AddDays(2);

            // Act

            var actual = PrefixService.BuildBlobPrefixes(from, to, Prefix);

            // Assert

            var expected = new List<string>
            {
                $"{Prefix}2018/10"
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GivenDayOrMoreDifferenceButLessThan28DaysAndDifferentYears_ThenLastMonthOfFromYearAndFirstMonthOfToYear()
        {
            // Arrange

            var from = new DateTime(2018, 12, 31, 23, 55, 15, DateTimeKind.Utc);
            var to = from.AddDays(2);

            // Act

            var actual = PrefixService.BuildBlobPrefixes(from, to, Prefix);

            // Assert

            var expected = new List<string>
            {
                $"{Prefix}2018/12",
                $"{Prefix}2019/01"
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Given28DayOrMoreMoreButLessThanYear_ThenAllTheWayToYear()
        {
            // Arrange

            var from = new DateTime(2018, 10, 6, 16, 7, 3, DateTimeKind.Utc);
            var to = from.AddDays(29);

            // Act

            var actual = PrefixService.BuildBlobPrefixes(from, to, Prefix);

            // Assert

            var expected = new List<string>
            {
                $"{Prefix}2018"
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Given28DayOrMoreButLessThanYearAndDifferentYear_ThenYearOfFromDateAndYearOfToDate()
        {
            // Arrange

            var from = new DateTime(2018, 12, 31, 23, 55, 15, DateTimeKind.Utc);
            var to = from.AddDays(29);

            // Act

            var actual = PrefixService.BuildBlobPrefixes(from, to, Prefix);

            // Assert

            var expected = new List<string>
            {
                $"{Prefix}2018",
                $"{Prefix}2019"
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GivenMultipleYears_ThenYearOfFromDateAndYearOfToDateAndAllYearsInBetween()
        {
            // Arrange

            var from = new DateTime(2018, 12, 31, 23, 55, 15, DateTimeKind.Utc);
            var to = from.AddYears(5);

            // Act

            var actual = PrefixService.BuildBlobPrefixes(from, to, Prefix);

            // Assert

            var expected = new List<string>
            {
                $"{Prefix}2018",
                $"{Prefix}2019",
                $"{Prefix}2020",
                $"{Prefix}2021",
                $"{Prefix}2022",
                $"{Prefix}2023"
            };

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
