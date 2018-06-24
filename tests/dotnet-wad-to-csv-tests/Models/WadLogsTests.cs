using DotNet.WadToCsv.Models;
using FluentAssertions;
using Xunit;

namespace DotNet.WadToCsv.Tests.Models
{
    public class WadLogsTests
    {
        [Fact]
        public void GivenCriticalLogMessage_WhenToString_ThenFormatLog()
        {
            // Arrange

            WadLogs log = new WadLogsBuilder().AsCriticalLog();

            // Act

            var actual = log.ToString();

            // Assert

            actual.Should().Be($"{WadLogsBuilder.ExpectedGenerated},Critical,\"{WadLogsBuilder.LogMessage}\"");
        }

        [Fact]
        public void GivenErrorLogMessage_WhenToString_ThenFormatLog()
        {
            // Arrange

            WadLogs log = new WadLogsBuilder().AsErrorLog();

            // Act

            var actual = log.ToString();

            // Assert

            actual.Should().Be($"{WadLogsBuilder.ExpectedGenerated},Error,\"{WadLogsBuilder.LogMessage}\"");
        }

        [Fact]
        public void GivenWarningLogMessage_WhenToString_ThenFormatLog()
        {
            // Arrange

            WadLogs log = new WadLogsBuilder().AsWarningLog();

            // Act

            var actual = log.ToString();

            // Assert

            actual.Should().Be($"{WadLogsBuilder.ExpectedGenerated},Warning,\"{WadLogsBuilder.LogMessage}\"");
        }

        [Fact]
        public void GivenInformationLogMessage_WhenToString_ThenFormatLog()
        {
            // Arrange

            WadLogs log = new WadLogsBuilder().AsInformationLog();

            // Act

            var actual = log.ToString();

            // Assert

            actual.Should().Be($"{WadLogsBuilder.ExpectedGenerated},Information,\"{WadLogsBuilder.LogMessage}\"");
        }

        [Fact]
        public void GivenVerboseLogMessage_WhenToString_ThenFormatLog()
        {
            // Arrange

            WadLogs log = new WadLogsBuilder().AsVerboseLog();

            // Act

            var actual = log.ToString();

            // Assert

            actual.Should().Be($"{WadLogsBuilder.ExpectedGenerated},Verbose,\"{WadLogsBuilder.VerboseLogMessage}\"");
        }

        [Fact]
        public void GivenNonExposedSettingRead_WhenToString_ThenAsIs()
        {
            // Arrange

            WadLogs log = new WadLogsBuilder().AsNonExposedSettingRead();

            // Act

            var actual = log.ToString();

            // Assert

            actual
                .Should()
                .Be($"{WadLogsBuilder.ExpectedGenerated},Verbose,\"{WadLogsBuilder.NonExposedSettingReadMessage}\"");
        }

        [Fact]
        public void GivenExposedSettingRead_WhenToString_ThenObfuscateSettingValue()
        {
            // Arrange

            WadLogs log = new WadLogsBuilder().AsExposedSettingRead();

            // Act

            var actual = log.ToString();

            // Assert

            actual
                .Should()
                .Be($"{WadLogsBuilder.ExpectedGenerated},Verbose,\"{WadLogsBuilder.ExpectedExposedSettingReadMessage}\"");
        }
    }
}
