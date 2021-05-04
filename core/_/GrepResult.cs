using System;

namespace YAGrep {
    public readonly struct GrepResult : IEquatable<string> {
        public bool IsMatch { get; }

        public string Line { get; }
        public int LineNumber { get; }

        public int MatchStart { get; }
        public int MatchEnd { get; }

        private GrepResult(bool isMatch, string line, int lineNumber = -1, int matchStart = -1, int matchEnd = -1) {
            IsMatch = isMatch;
            Line = line;
            LineNumber = lineNumber;
            MatchStart = matchStart;
            MatchEnd = matchEnd;
        }

        public static GrepResult Failure(string line) => new(isMatch: false, line: line);

        public static GrepResult Success(string line, int lineNumber, int matchStart, int matchEnd) =>
            new(isMatch: true, line: line, lineNumber, matchStart, matchEnd);

        public bool Equals(string other) => other == Line;

        public override string ToString() => Line;
    }
}
