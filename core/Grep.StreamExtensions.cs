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

            #region should cancel?
            if (cancel.Value.IsCancellationRequested)
                if (options.SilentCancel) return EndReason.Canceled;
                else cancel.Value.ThrowIfCancellationRequested();
            #endregion

            var line = await haystack.ReadLineAsync();
            if (line == null) return EndReason.EmptyInput;

            var lineIndex = 0;
            var isMatch = MatchFunction.For(needle, options);
            string nextLine;
            while ((nextLine = await haystack.ReadLineAsync()) != null) {
                #region should cancel?
                if (cancel.Value.IsCancellationRequested)
                    if (options.SilentCancel) return EndReason.Canceled;
                    else cancel.Value.ThrowIfCancellationRequested();
                #endregion

                var match = isMatch(line, lineIndex);
                lineIndex++;
                line = nextLine;

                if (!match.IsMatch) continue;
                if (!processAndContinue(match)) return EndReason.Interrupted;
            }

            //reading last line
            var lastMatch = isMatch(line, lineIndex);
            if (!lastMatch.IsMatch) return EndReason.EndOfInput;
            processAndContinue(lastMatch);

            return EndReason.EndOfInput;
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
                result.Add(match);
                return top < 0 || result.Count < top;
            }

            await haystack.Grep(needle, collect, options);
            return result;
        }
    }
}
