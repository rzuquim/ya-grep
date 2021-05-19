using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace YAGrep {
    public static class GrepOverFileExtensions {
        public static async Task<GrepStatistics> Grep(
                this string fileFullPath, string needle, Func<GrepResult, bool> processAndContinue,
                GrepOptions? options = null, CancellationToken? cancel = null) {
            using var stream = File.OpenRead(fileFullPath);
            return await stream.Grep(needle, processAndContinue, options, cancel);
        }

        public static async Task<GrepStatistics> Grep(
                this string fileFullPath, string needle, Action<GrepResult> process,
                GrepOptions? options = null, CancellationToken? cancel = null) =>
            await Grep(fileFullPath, needle, r => { process(r); return true; }, options, cancel);

        public static async Task<IEnumerable<GrepResult>> Grep(
                this string fileFullPath, string needle, GrepOptions? options = null) {
            using var stream = File.OpenRead(fileFullPath);
            return await stream.Grep(needle, options);
        }
    }
}
