using System;

namespace YAGrep {
    public partial class MatchFunction {
        public static Func<Line, int, GrepResult> For(string needle, GrepOptions options) {
            // TODO: regexp support
            return NaiveStringContains(needle, options);
        }
    }
}
