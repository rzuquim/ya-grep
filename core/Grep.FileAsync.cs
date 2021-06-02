using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace YAGrep {
    public static class GrepOverFileExtensionsAsync {
        public static async Task<GrepStatistics> Grep(
                this string fileFullPath, string needle,
                Func<GrepResult, CancellationToken?, Task<bool>> processAndContinue,
                GrepOptions? options = null, CancellationToken? cancel = null) {
            options ??= GrepOptions.Default;
            using var stream = File.OpenRead(fileFullPath);
            using var reader = new StreamReader(stream, options.Encoding);
            return await reader.Grep(needle, processAndContinue, options, cancel);
        }

        public static async Task<GrepStatistics> Grep(
                this string fileFullPath, string needle,
                Func<GrepResult, Task<bool>> processAndContinue,
                GrepOptions? options = null, CancellationToken? cancel = null) =>
            await Grep(fileFullPath, needle, async (match, _) => await processAndContinue(match), options, cancel);

        public static async Task<GrepStatistics> Grep(
                this string fileFullPath, string needle,
                Func<GrepResult, Task> processAndContinue,
                GrepOptions? options = null, CancellationToken? cancel = null) =>
            await Grep(
                fileFullPath, needle, async (match, _) => { await processAndContinue(match); return true; },
                options, cancel);
    }
}
