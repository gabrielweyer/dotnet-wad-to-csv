using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DotNet.WadToCsv.Services;
using DotNet.WadToCsv.Validation;
using McMaster.Extensions.CommandLineUtils;

namespace DotNet.WadToCsv
{
    class Program
    {
        [Required]
        [Iso8601TimeDuration]
        [Option(ShortName = "l", LongName = "last",
            Description = "ISO 8601 duration, substracted from the current UTC time")]
        public string Last { get; }

        [Required]
        [WritableFile]
        [Option(ShortName = "o", LongName = "output", Description = "Output file path")]
        public string OutputFilePath { get; }

        private static readonly CancellationTokenSource Cts = new CancellationTokenSource();
        private static readonly CancellationToken Token = Cts.Token;

        static Task<int> Main(string[] args) => CommandLineApplication.ExecuteAsync<Program>(args);

        private async Task OnExecuteAsync()
        {
            var fullPath = Path.GetFullPath(OutputFilePath);
            var last = Last.ParseIso8601TimeDuration();

            var sas = Prompt.GetPassword("Shared Access Signature:", ConsoleColor.White, ConsoleColor.DarkBlue);

            Console.CancelKeyPress += ConsoleOnCancelKeyPress;

            try
            {
                var from = DateTime.UtcNow - last;

                WriteLine($"Querying storage account '{GetStorageAccountName(sas)}' from '{from:u}' to 'Now'");

                var repository = new Repository(sas);
                var logs = await repository.GetLogsAsync(from, Token);

                using (var outputFile = File.CreateText(fullPath))
                {
                    outputFile.WriteLine("Generated,Level,Message");
                    foreach (var log in logs)
                    {
                        outputFile.WriteLine(log);
                    }
                }

                Console.WriteLine();
                WriteLine("Done");
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

        private static void WriteLine(string line)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(line);
            Console.ResetColor();
        }

        private static string GetStorageAccountName(string connectionString)
        {
            var lastIndex = connectionString.LastIndexOf(':');

            if (lastIndex == -1) return null;

            var storageAccountUri = connectionString.Substring(lastIndex + 3);

            var firstindex = storageAccountUri.IndexOf('.');

            return firstindex == -1 ? null : storageAccountUri.Substring(0, firstindex);
        }

        private static void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs consoleCancelEventArgs)
        {
            Console.WriteLine("ConsoleCancelEvent received => Cancelling token");
            consoleCancelEventArgs.Cancel = true;
            Cts.Cancel();
        }
    }
}
