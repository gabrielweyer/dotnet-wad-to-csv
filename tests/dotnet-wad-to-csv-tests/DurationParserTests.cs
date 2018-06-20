using System;
using FluentAssertions;
using Xunit;

namespace DotNet.WadToCsv.Tests
{
    public class DurationParserTests
    {
        [Theory]
        [InlineData("26h")]
        [InlineData("26H")]
        public void GivenOnlyHours_WhenParse_ThenExpectedTimeSpan(string input)
        {
            // Act

            var actualValue = input.ParseIso8601TimeDuration();

            // Assert

            actualValue.Should().Be(TimeSpan.FromHours(26));
        }

        [Theory]
        [InlineData("80m")]
        [InlineData("80M")]
        public void GivenOnlyMinutes_WhenParse_ThenExpectedTimeSpan(string input)
        {
            // Act

            var actualValue = input.ParseIso8601TimeDuration();

            // Assert

            actualValue.Should().Be(TimeSpan.FromMinutes(80));
        }

        [Theory]
        [InlineData("75s")]
        [InlineData("75S")]
        public void GivenOnlySeconds_WhenParse_ThenExpectedTimeSpan(string input)
        {
            // Act

            var actualValue = input.ParseIso8601TimeDuration();

            // Assert

            actualValue.Should().Be(TimeSpan.FromSeconds(75));
        }

        [Theory]
        [InlineData("1H5M3S", 3600 + 5 * 60 + 3)]
        [InlineData("2H8S", 2 * 3600 + 8)]
        [InlineData("2H7M", 2 * 3600 + 7 * 60)]
        [InlineData("9M2S", 9 * 60 + 2)]
        public void GivenHoursMinutesSecondsCombination_WhenParse_ThenExpectedTimeSpan(string input, int expectedSecondDuration)
        {
            // Act

            var actualValue = input.ParseIso8601TimeDuration();

            // Assert

            actualValue.Should().Be(TimeSpan.FromSeconds(expectedSecondDuration));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void GivenNullOrEmptyDuration_WhenParse_ThenThrows(string input)
        {
            // Act

            var actualException = Record.Exception(() => input.ParseIso8601TimeDuration());

            // Assert

            Assert.IsType<FormatException>(actualException);
        }

        [Theory]
        [InlineData("P3Y6M4DT12H30M5S")]
        public void GivenInvalidDuration_WhenParse_ThenThrows(string input)
        {
            // Act

            var actualException = Record.Exception(() => input.ParseIso8601TimeDuration());

            // Assert

            Assert.IsType<FormatException>(actualException);
        }
    }
}
