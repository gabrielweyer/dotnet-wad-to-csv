using System;
using FluentAssertions;
using Xunit;

namespace DotNet.WadToCsv.Tests
{
    public class DateTimeParserTests
    {
        [Fact]
        public void GivenDateTime_WhenTryParseIso8601DateTime_ThenExpectedDateTime()
        {
            // Arrange

            const string input = "2018-06-24T21:03:05";

            // Act

            var actualSuccess = input.TryParseIso8601DateTime(out var actualParsedDate);

            // Assert

            var expectedParsedDate = new DateTime(2018, 6, 24, 21, 3, 5, DateTimeKind.Utc);

            Assert.True(actualSuccess);
            actualParsedDate.Should().Be(expectedParsedDate);
        }

        [Fact]
        public void GivenDateOnly_WhenTryParseIso8601DateTime_ThenExpectedDateTime()
        {
            // Arrange

            const string input = "2018-06-24";

            // Act

            var actualSuccess = input.TryParseIso8601DateTime(out var actualParsedDate);

            // Assert

            var expectedParsedDate = new DateTime(2018, 6, 24, 0, 0, 0, DateTimeKind.Utc);

            Assert.True(actualSuccess);
            actualParsedDate.Should().Be(expectedParsedDate);
        }

        [Fact]
        public void GivenDateAndHoursOnly_WhenTryParseIso8601DateTime_ThenExpectedDateTime()
        {
            // Arrange

            const string input = "2018-06-24T15";

            // Act

            var actualSuccess = input.TryParseIso8601DateTime(out var actualParsedDate);

            // Assert

            var expectedParsedDate = new DateTime(2018, 6, 24, 15, 0, 0, DateTimeKind.Utc);

            Assert.True(actualSuccess);
            actualParsedDate.Should().Be(expectedParsedDate);
        }
    }
}
