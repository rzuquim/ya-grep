using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace YAGrep {
    public static class StreamExtensions {
        public static async Task<EndReason> Grep(
                this StreamReader haystack, string needle, Func<GrepResult, bool> processAndContinue,
                GrepOptions? options = null, CancellationToken? cancel = null) {
            options ??= GrepOptions.Default;
            cancel ??= CancellationToken.None;

            var lineReader = new LineReader(haystack, options);
            var isMatch = MatchFunction.For(needle, options);
            var lineIndex = 0;
            var anyInput = false;

            Line line;
            while ((line = await lineReader.NextLine()).Valid()) {
                anyInput = true;
                if (cancel.Value.IsCancellationRequested)
                    if (options.SilentCancel) return EndReason.Canceled;
                    else cancel.Value.ThrowIfCancellationRequested();

                var match = isMatch(line, lineIndex);
                lineIndex++;

                if (match.IsMatch && !processAndContinue(match)) return EndReason.Interrupted;
            }

            return anyInput ? EndReason.EndOfInput : EndReason.EmptyInput;
        }

        public static async Task<EndReason> Grep(
                this Stream haystack, string needle, Action<GrepResult> process,
                GrepOptions? options = null, CancellationToken? cancel = null) =>
           await Grep(haystack, needle, r => { process(r); return true; }, options, cancel);

        public static async Task<EndReason> Grep(
                this Stream haystack, string needle, Func<GrepResult, bool> processAndContinue,
                GrepOptions? options = null, CancellationToken? cancel = null) {
            options ??= GrepOptions.Default;
            using var reader = new StreamReader(haystack, options.Encoding);
            return await reader.Grep(needle, processAndContinue, options, cancel);
        }

        public static async Task<EndReason> Grep(
                this StreamReader haystack, string needle, Action<GrepResult> process,
                GrepOptions? options = null, CancellationToken? cancel = null) =>
            await Grep(haystack, needle, r => { process(r); return true; }, options, cancel);

        public static async Task<IEnumerable<GrepResult>> Grep(
                this Stream haystack, string needle, int top = -1, GrepOptions? options = null) {
            var result = new List<GrepResult>();

            bool collect(GrepResult match) {
                result.Add(match.Clone());
                return top < 0 || result.Count < top;
            }

            await haystack.Grep(needle, collect, options);
            return result;
        }
    }
}
