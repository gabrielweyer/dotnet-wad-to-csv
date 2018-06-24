using System;

namespace DotNet.WadToCsv.Models
{
    public class WadLogs
    {
        public DateTime Generated { get; set; }
        public string Message { get; set; }
        public int Level { get; set; }

        private const int TwoDoubleQuoteLength = 2;

        private const int MessageEventLengthBeforeMessage = 33;
        private const int MessageEventLengthAfterMessage = 23;

        private const int MessageEventMinimalLenght =
            MessageEventLengthBeforeMessage + MessageEventLengthAfterMessage + TwoDoubleQuoteLength;

        private const int DirectWriteLengthBeforeMessage = 32;
        private const int DirectWriteLengthAfterMessage = 0;

        private const int DirectWriteMinimalLenght =
            DirectWriteLengthBeforeMessage + DirectWriteLengthAfterMessage + TwoDoubleQuoteLength;

        public override string ToString()
        {
            var message = FormatLog();

            return string.IsNullOrWhiteSpace(message) ? null : $"{FormatGenerated()},{FormatLevel()},{message}";

            string FormatLevel()
            {
                switch ((WadLogLevel) Level)
                {
                    case WadLogLevel.Critical:
                        return "Critical";
                    case WadLogLevel.Error:
                        return "Error";
                    case WadLogLevel.Information:
                        return "Information";
                    case WadLogLevel.Warning:
                        return "Warning";
                    case WadLogLevel.Verbose:
                        return "Verbose";
                    default:
                        return "Undefined";
                }
            }

            string FormatLog()
            {
                var isMessageEvent = IsMessageEvent();
                var isDirectWrite = IsDirectWrite();

                if (isMessageEvent)
                {
                    return FormatMessage(MessageEventLengthBeforeMessage, MessageEventLengthAfterMessage);
                }

                if (isDirectWrite)
                {
                    return FormatMessage(DirectWriteLengthBeforeMessage, DirectWriteLengthAfterMessage);
                }

                return null;
            }

            string FormatGenerated()
            {
                return Generated.ToString("yyyy-MM-ddTHH:mm:ss.fffT");
            }
        }

        private bool IsMessageEvent()
        {
            return Message.Length >= MessageEventMinimalLenght && Message.StartsWith("EventName=\"MessageEvent\"");
        }

        private bool IsDirectWrite()
        {
            return Message.Length >= DirectWriteMinimalLenght && Message.StartsWith("EventName=\"DirectWrite\"");
        }

        private string FormatMessage(int lengthBeforeMessage, int lengthAfterMessage)
        {
            var formattedMessage = Message.Substring(
                lengthBeforeMessage,
                Message.Length - (lengthBeforeMessage + lengthAfterMessage));

            return ObfuscateExposedSettingRead(formattedMessage);
        }

        private string ObfuscateExposedSettingRead(string formattedMessage)
        {
            if (Message.StartsWith("EventName=\"DirectWrite\" Message=\"Getting \""))
            {
                const string settingReadMarker = ": PASS (";
                var indexOfSettingRead = formattedMessage.IndexOf(settingReadMarker, StringComparison.Ordinal);

                if (indexOfSettingRead > -1)
                {
                    return formattedMessage.Substring(0, indexOfSettingRead + settingReadMarker.Length) + "*).\"";
                }
            }

            return formattedMessage;
        }
    }
}
