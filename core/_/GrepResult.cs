using System;

namespace YAGrep {
    public readonly struct GrepResult : IEquatable<GrepResult> {
        public bool IsMatch { get; }

        public char[] Line { get; }
        /// <summary> one based, line index </summary>
        public int LineNumber { get; }

        public int MatchStart { get; }
        public int MatchEnd { get; }

        private GrepResult(bool isMatch, char[] line, int lineNumber = -1, int matchStart = -1, int matchEnd = -1) {
            IsMatch = isMatch;
            Line = line;
            LineNumber = lineNumber;
            MatchStart = matchStart;
            MatchEnd = matchEnd;
        }

        public static GrepResult Failure(char[] line) => new(isMatch: false, line: line);

        public static GrepResult Success(string line, int lineNumber, int matchStart, int matchEnd) =>
            new(isMatch: true, line: line.ToCharArray(), lineNumber, matchStart, matchEnd);

        public static GrepResult Success(char[] line, int lineNumber, int matchStart, int matchEnd) =>
            new(isMatch: true, line: line, lineNumber, matchStart, matchEnd);

        public bool Equals(GrepResult other) =>
            IsMatch == other.IsMatch &&
            LineNumber == other.LineNumber &&
            MatchStart == other.MatchStart &&
            MatchEnd == other.MatchEnd;

        public override bool Equals(object? obj) => obj is GrepResult other && Equals(other);

        public override int GetHashCode() {
            unchecked {
                var hashCode = IsMatch.GetHashCode();
                hashCode = (hashCode * 397) ^ LineNumber;
                hashCode = (hashCode * 397) ^ MatchStart;
                hashCode = (hashCode * 397) ^ MatchEnd;
                return hashCode;
            }
        }

        public override string ToString() => new { IsMatch, Line, LineNumber, MatchStart, MatchEnd }.ToString();
    }
}
