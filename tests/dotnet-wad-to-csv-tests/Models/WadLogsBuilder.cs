using System;
using DotNet.WadToCsv.Models;

namespace DotNet.WadToCsv.Tests.Models
{
    public class WadLogsBuilder
    {
        public const string ExpectedGenerated = "2018-06-03T23:55:22.189T";

        public const string LogMessage = "Oh Hello!";

        private static readonly string FullLogMessage =
            $"EventName=\"MessageEvent\" Message=\"{LogMessage}\" TraceSource=\"w3wp.exe\"";

        public const string VerboseLogMessage = "Oh Hi!";

        private static readonly string FullVerboseLogMessage =
            $"EventName=\"DirectWrite\" Message=\"{VerboseLogMessage}\"";

        public const string ExposedSettingReadMessage = "Getting \"ApiKey\" from ServiceRuntime: PASS (TopSecret).";
        public const string ExpectedExposedSettingReadMessage = "Getting \"ApiKey\" from ServiceRuntime: PASS (*).";

        public static readonly string FullExposedSettingReadMessage =
            $"EventName=\"DirectWrite\" Message=\"{ExposedSettingReadMessage}\"";

        public const string NonExposedSettingReadMessage = "Getting \"ApiKey\" from ServiceRuntime: PASS.";

        private static readonly string FullNonExposedSettingReadMessage =
            $"EventName=\"DirectWrite\" Message=\"{NonExposedSettingReadMessage}\"";

        private string _message = FullLogMessage;
        private int _level = (int) WadLogLevel.Verbose;

        public WadLogsBuilder AsExposedSettingRead()
        {
            _message = FullExposedSettingReadMessage;
            _level = (int) WadLogLevel.Verbose;

            return this;
        }

        public WadLogsBuilder AsNonExposedSettingRead()
        {
            _message = FullNonExposedSettingReadMessage;
            _level = (int) WadLogLevel.Verbose;

            return this;
        }

        public WadLogsBuilder AsCriticalLog()
        {
            _message = FullLogMessage;
            _level = (int) WadLogLevel.Critical;

            return this;
        }

        public WadLogsBuilder AsErrorLog()
        {
            _message = FullLogMessage;
            _level = (int) WadLogLevel.Error;

            return this;
        }

        public WadLogsBuilder AsWarningLog()
        {
            _message = FullLogMessage;
            _level = (int) WadLogLevel.Warning;

            return this;
        }

        public WadLogsBuilder AsInformationLog()
        {
            _message = FullLogMessage;
            _level = (int) WadLogLevel.Information;

            return this;
        }

        public WadLogsBuilder AsVerboseLog()
        {
            _message = FullVerboseLogMessage;
            _level = (int) WadLogLevel.Verbose;

            return this;
        }

        public WadLogsBuilder WithLevel(WadLogLevel level)
        {
            _level = (int) level;

            return this;
        }

        public static implicit operator WadLogs(WadLogsBuilder b)
        {
            if (b == null) return null;

            return new WadLogs
            {
                Generated = new DateTime(2018, 6, 3, 23, 55, 22, 189),
                Level = b._level,
                Message = b._message
            };
        }
    }
}
