using System;
using DotNet.AzureDiagnostics.Core.Parsers;
using FluentAssertions;
using Xunit;

namespace DotNet.WadToCsv.Tests.Parsers
{
    public class RangeParserTests
    {
        [Fact]
        public void GivenNoLastAndNoFrom_WhenTryGetRange_ThenInvalid()
        {
            // Arrange

            const string last = null;
            const string from = null;
            const string to = null;

            // Act

            var actual = RangeParser.TryGetRange(last, from, to, out _);

            // Assert

            Assert.NotNull(actual);
        }

        [Fact]
        public void GivenBothLastAndFrom_WhenTryGetRange_ThenInvalid()
        {
            // Arrange

            const string last = "5M";
            const string from = "2018-06-24";
            const string to = null;

            // Act

            var actual = RangeParser.TryGetRange(last, from, to, out _);

            // Assert

            Assert.NotNull(actual);
        }

        [Fact]
        public void GivenToWithoutFrom_WhenTryGetRange_ThenInvalid()
        {
            // Arrange

            const string last = null;
            const string from = null;
            const string to = "2018-06-24";

            // Act

            var actual = RangeParser.TryGetRange(last, from, to, out _);

            // Assert

            Assert.NotNull(actual);
        }

        [Fact]
        public void GivenLast_WhenTryGetRange_ThenExpected()
        {
            // Arrange

            const string last = "5M";
            const string from = null;
            const string to = null;

            // Act

            var actual = RangeParser.TryGetRange(last, from, to, out var actualRange);

            // Assert

            Assert.Null(actual);
            Assert.NotNull(actualRange);

            actualRange.From.Should().NotBe(default(DateTime));
            actualRange.To.Should().BeNull();
        }

        [Fact]
        public void GivenFrom_WhenTryGetRange_ThenExpected()
        {
            // Arrange

            const string last = null;
            const string from = "2018-06-24";
            const string to = null;

            // Act

            var actual = RangeParser.TryGetRange(last, from, to, out var actualRange);

            // Assert

            Assert.Null(actual);
            Assert.NotNull(actualRange);

            actualRange.From.Should().NotBe(default(DateTime));
            actualRange.To.Should().BeNull();
        }

        [Fact]
        public void GivenFromAndTo_WhenTryGetRange_ThenExpected()
        {
            // Arrange

            const string last = null;
            const string from = "2018-06-24";
            const string to = "2018-06-25";

            // Act

            var actual = RangeParser.TryGetRange(last, from, to, out var actualRange);

            // Assert

            Assert.Null(actual);
            Assert.NotNull(actualRange);

            actualRange.From.Should().NotBe(default(DateTime));
            actualRange.To.Should().NotBeNull();
        }
    }
}
