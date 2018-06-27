using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DotNet.AzureDiagnostics.Core.Helpers;
using DotNet.AzureDiagnostics.Core.Parsers;
using DotNet.AzureDiagnostics.Core.Validation;
using DotNet.WadToCsv.Services;
using McMaster.Extensions.CommandLineUtils;

namespace DotNet.WadToCsv
{
    class Program
    {
        [Iso8601TimeDuration]
        [Option(ShortName = "l", LongName = "last",
            Description = "ISO 8601 time duration, substracted from the current UTC time")]
        public string Last { get; }

        [Iso8601DateTime]
        [Option(ShortName = "f", LongName = "from",
            Description = "ISO 8601 date time in UTC, time can be omitted")]
        public string From { get; }

        [Iso8601DateTime]
        [Option(ShortName = "t", LongName = "to",
            Description = "ISO 8601 date time in UTC, time can be omitted")]
        public string To { get; }

        [Required]
        [WritableFile]
        [Option(ShortName = "o", LongName = "output", Description = "Required. Output file path")]
        public string OutputFilePath { get; }

        private static readonly CancellationTokenSource Cts = new CancellationTokenSource();
        private static readonly CancellationToken Token = Cts.Token;

        static Task<int> Main(string[] args) => CommandLineApplication.ExecuteAsync<Program>(args);

        private async Task OnExecuteAsync()
        {
            var fullPath = Path.GetFullPath(OutputFilePath);

            var getRangeResult = RangeParser.TryGetRange(Last, From, To, out var range);

            if (!string.IsNullOrEmpty(getRangeResult?.ErrorMessage))
            {
                ConsoleHelper.WriteError(getRangeResult.ErrorMessage);
                return;
            }

            var sas = Prompt.GetPassword("Shared Access Signature:", ConsoleColor.White, ConsoleColor.DarkBlue);

            Console.CancelKeyPress += ConsoleOnCancelKeyPress;

            try
            {
                ConsoleHelper.WriteDebug($"Querying storage account '{StorageAccountHelper.GetStorageAccountName(sas)}' from {range}");

                var repository = new Repository(sas);
                var logs = await repository.GetLogsAsync(range, Token);

                using (var outputFile = File.CreateText(fullPath))
                {
                    outputFile.WriteLine("Generated,Level,Message");
                    foreach (var log in logs)
                    {
                        outputFile.WriteLine(log);
                    }
                }

                Console.WriteLine();
                ConsoleHelper.WriteDebug("Done");
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception: {0}", e.GetType());
                Console.WriteLine("Message: {0}", e.Message);
                Console.WriteLine("StackTrace:");
                Console.WriteLine(e.Demystify().StackTrace);
            }
            finally
            {
                Console.ResetColor();
            }
        }

        private static void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs consoleCancelEventArgs)
        {
            Console.WriteLine("ConsoleCancelEvent received => Cancelling token");
            consoleCancelEventArgs.Cancel = true;
            Cts.Cancel();
        }
    }
}
