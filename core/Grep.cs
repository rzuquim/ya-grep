using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace YAGrep {
    public static class StreamExtensions {
        public static async Task<EndReason> Grep(
                this Stream haystack, string needle, Func<GrepResult, bool> processAndContinue,
                GrepOptions? options = null, CancellationToken? cancel = null) {
            options ??= GrepOptions.Default;
            using var reader = new StreamReader(haystack, options.Encoding);
            return await reader.Grep(needle, processAndContinue, options, cancel);
        }

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

            var iteration = 0;
            var isMatch = MatchFunction.For(needle, options);
            string nextLine;
            while ((nextLine = await haystack.ReadLineAsync()) != null) {
                #region should cancel?
                if (cancel.Value.IsCancellationRequested)
                    if (options.SilentCancel) return EndReason.Canceled;
                    else cancel.Value.ThrowIfCancellationRequested();
                #endregion

                var match = isMatch(line, iteration);
                if (!match.IsMatch) {
                    line = nextLine;
                    continue;
                }

                var shouldContinue = processAndContinue(match);

                line = nextLine;
                iteration++;

                if (!shouldContinue) return EndReason.Interrupted;
            }

            //reading last line
            isMatch(line, iteration);
            return EndReason.EndOfInput;
        }
    }
}
