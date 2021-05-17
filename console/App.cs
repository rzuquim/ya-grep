using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using static System.Console;

namespace YAGrep {
    internal class Program {
        private static async Task Main(string[] args) {
            var rootCommand = new RootCommand {
                Options.Regexp,
                Options.File,
                Options.PrintStatistics
            };

            rootCommand.Description = "Yet another sub-optimal grep implementation.";
            rootCommand.Handler = CommandHandler.Create<string, FileInfo, bool>(
                async (regexp, file, statistics) => {
                    var totalMatches = 0;
                    var stopWatch = Stopwatch.StartNew();

                    await file.FullName.Grep(regexp,
                                             r => {
                                                 totalMatches++;
                                                 WriteLine("[line {0}]: {1}", r.LineNumber, r.Line.AsString());
                                             });

                    stopWatch.Stop();
                    if (statistics)
                        WriteLine($"Total matches: {totalMatches}. Runtime: {stopWatch.ElapsedMilliseconds} ms");
                });

            Environment.ExitCode = await rootCommand.InvokeAsync(args);
        }
    }
}
