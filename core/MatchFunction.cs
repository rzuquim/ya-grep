using System;

namespace YAGrep {
    internal partial class MatchFunction {
        public static Func<string, int, GrepResult> For(string needle, GrepOptions options) {
            // TODO: regexp support
            return NaiveStringContains(needle, options);
        }
    }
}
