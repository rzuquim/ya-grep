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
            };

            rootCommand.Description = "Yet another sub-optimal grep implementation.";
            rootCommand.Handler = CommandHandler.Create<string, FileInfo>(
                async (regexp, file) =>
                    await file.FullName.Grep(regexp, r => WriteLine("[line {0}]: {1}", r.LineNumber, r.Line))
            );

            Environment.ExitCode = await rootCommand.InvokeAsync(args);

            WriteLine("Press any key to exit...");
            ReadLine();
        }
    }
}
