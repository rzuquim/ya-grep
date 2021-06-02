using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Text;
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
                Options.MaxCount,
                Options.OutputFile
            };

            rootCommand.Description = "Yet another sub-optimal grep implementation.";
            rootCommand.Handler = CommandHandler.Create<string, FileInfo, bool, bool, int, FileInfo>(
                async (regexp, file, statistics, trim, maxCount, outputFile) => {
                    var options = new GrepOptions(trim: trim, maxCount: maxCount, captureStatistics: statistics);
                    var result = outputFile == null
                        ? await file.FullName.Grep(regexp,
                                                   r => WriteLine("[line {0}]{1}", r.LineNumber, r.Line.AsString()),
                                                   options)
                        : await GrepAndSaveOnFile(file, regexp, outputFile, options);

                    if (statistics) {
                        WriteLine("------------------------");
                        WriteLine("------ STATISTICS ------");
                        WriteLine("------------------------");

                        WriteLine(result);
                    }
                });

            Environment.ExitCode = await rootCommand.InvokeAsync(args);
        }

        private static async Task<GrepStatistics> GrepAndSaveOnFile(
                FileSystemInfo input, string regexp, FileInfo output, GrepOptions options) {
            await using var target = File.Create(output.FullName);
            await using var writer = new StreamWriter(target, Encoding.UTF8) { NewLine = "\n", AutoFlush = true };
            return await input.FullName.Grep(regexp, async r => await r.FlushInto(writer, autoFlush: false), options);
        }
    }
}
