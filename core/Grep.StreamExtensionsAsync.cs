using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static YAGrep.EndReason;

namespace YAGrep {
    public static class StreamExtensionsAsync {
        public static async Task<GrepStatistics> Grep(
                this StreamReader haystack, string needle,
                Func<GrepResult, CancellationToken?, Task<bool>> processAndContinue,
                GrepOptions? options = null, CancellationToken? cancel = null) {
            options ??= GrepOptions.Default;
            cancel ??= CancellationToken.None;

            var lineReader = new LineReader(haystack, options);
            var isMatch = MatchFunction.For(needle, options);
            var statistics = GrepStatistics.Start(options.CaptureStatistics);

            Line line;
            while ((line = await lineReader.NextLine()).Valid()) {
                if (cancel.Value.IsCancellationRequested)
                    if (options.SilentCancel) return statistics.EndRun(Canceled);
                    else cancel.Value.ThrowIfCancellationRequested();

                var match = isMatch(line, statistics.LineIndex());

                if (!match.IsMatch) continue;

                statistics.RegisterMatch();
                if (!await processAndContinue(match, cancel)) return statistics.EndRun(Interrupted);
            }

            return statistics.TotalLinesRead > 0
                ? statistics.EndRun(EndOfInput)
                : statistics.EndRun(EmptyInput);
        }

        public static async Task<GrepStatistics> Grep(
                this StreamReader haystack, string needle,
                Func<GrepResult, Task<bool>> processAndContinue,
                GrepOptions? options = null, CancellationToken? cancel = null) =>
            await Grep(haystack, needle, async (match, _) => await processAndContinue(match), options, cancel);

        public static async Task<GrepStatistics> Grep(
                this StreamReader haystack, string needle,
                Func<GrepResult, Task> processAndContinue,
                GrepOptions? options = null, CancellationToken? cancel = null) =>
            await Grep(
                haystack, needle, async (match, _) => { await processAndContinue(match); return true; }, options,
                cancel);

        public static async Task<GrepStatistics> Grep(
                this Stream haystack, string needle,
                Func<GrepResult, Task<bool>> processAndContinue,
                GrepOptions? options = null, CancellationToken? cancel = null) {
            options ??= GrepOptions.Default;
            using var reader = new StreamReader(haystack, options.Encoding);
            return await Grep(reader, needle, async (match, _) => await processAndContinue(match), options, cancel);
        }

        public static async Task<GrepStatistics> Grep(
                this Stream haystack, string needle,
                Func<GrepResult, Task> processAndContinue,
                GrepOptions? options = null, CancellationToken? cancel = null) {
            options ??= GrepOptions.Default;
            using var reader = new StreamReader(haystack, options.Encoding);
            return await Grep(
                reader, needle, async (match, _) => { await processAndContinue(match); return true; }, options, cancel);
        }
    }
}
