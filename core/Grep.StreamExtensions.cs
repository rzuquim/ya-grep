using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static YAGrep.EndReason;

namespace YAGrep {
    public static class StreamExtensions {
        public static async Task<GrepStatistics> Grep(
                this StreamReader haystack, string needle, Func<GrepResult, bool> processAndContinue,
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
                if (!processAndContinue(match)) return statistics.EndRun(Interrupted);
            }

            return statistics.TotalLinesRead > 0
                ? statistics.EndRun(EndOfInput)
                : statistics.EndRun(EmptyInput);
        }

        public static async Task<GrepStatistics> Grep(
                this Stream haystack, string needle, Action<GrepResult> process,
                GrepOptions? options = null, CancellationToken? cancel = null) =>
           await Grep(haystack, needle, r => { process(r); return true; }, options, cancel);

        public static async Task<GrepStatistics> Grep(
                this Stream haystack, string needle, Func<GrepResult, bool> processAndContinue,
                GrepOptions? options = null, CancellationToken? cancel = null) {
            options ??= GrepOptions.Default;
            using var reader = new StreamReader(haystack, options.Encoding);
            return await reader.Grep(needle, processAndContinue, options, cancel);
        }

        public static async Task<GrepStatistics> Grep(
                this StreamReader haystack, string needle, Action<GrepResult> process,
                GrepOptions? options = null, CancellationToken? cancel = null) =>
            await Grep(haystack, needle, r => { process(r); return true; }, options, cancel);

        public static async Task<IEnumerable<GrepResult>> Grep(
                this Stream haystack, string needle, GrepOptions? options = null) {
            options ??= GrepOptions.Default;
            var result = new List<GrepResult>();

            bool collect(GrepResult match) {
                result.Add(match.Clone());
                return options.MaxCount < 0 || result.Count < options.MaxCount;
            }

            await haystack.Grep(needle, collect, options);
            return result;
        }
    }
}
