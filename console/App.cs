using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;
using static System.Console;

namespace YAGrep {
    internal class Program {
        private static async Task Main(string[] args) {
            var rootCommand = new RootCommand {
                Options.Regexp,
                Options.File,
                Options.PrintStatistics,
                Options.Trim,
                Options.MaxCount
            };

            rootCommand.Description = "Yet another sub-optimal grep implementation.";
            rootCommand.Handler = CommandHandler.Create<string, FileInfo, bool, bool, int>(
                async (regexp, file, statistics, trim, maxCount) => {
                    var result = await file.FullName.Grep(regexp,
                                             r => WriteLine("[line {0}]{1}", r.LineNumber, r.Line.AsString()),
                                             new GrepOptions(trim: trim,
                                                             maxCount: maxCount,
                                                             captureStatistics: statistics));

                    if (statistics) {
                        WriteLine("------------------------");
                        WriteLine("------ STATISTICS ------");
                        WriteLine("------------------------");

                        WriteLine(result);
                    }
                });

            Environment.ExitCode = await rootCommand.InvokeAsync(args);
        }
    }
}
