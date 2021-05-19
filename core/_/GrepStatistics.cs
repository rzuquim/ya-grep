using System;
using System.Diagnostics;

namespace YAGrep {
    public class GrepStatistics {
        private readonly Stopwatch? _stopwatch;
        private int _currentLineIndex = -1;

        public bool Enabled { get; }
        public int MatchesCount { get; private set; }
        public int TotalLinesRead => _currentLineIndex + 1;
        public TimeSpan Elapsed => _stopwatch?.Elapsed ?? TimeSpan.Zero;
        public EndReason EndReason { get; private set; }

        private GrepStatistics(bool enabled, Stopwatch? stopwatch = null) {
            Enabled = enabled;
            _stopwatch = stopwatch;
        }

        public void RegisterMatch() => MatchesCount++;

        public int LineIndex() {
            if (_currentLineIndex < 0) _currentLineIndex = 0;
            return _currentLineIndex++;
        }

        public GrepStatistics EndRun(EndReason endReason) {
            EndReason = endReason;
            if (!Enabled) return this;
            _stopwatch?.Stop();
            return this;
        }

        public static GrepStatistics Start(bool enabled) {
            return enabled
                ? new GrepStatistics(enabled: true, Stopwatch.StartNew())
                : new GrepStatistics(enabled: false);
        }

        public override string ToString() =>
            !Enabled
                ? new { EndReason }.ToString()
                : new {
                    EndReason,
                    MatchesCount,
                    TotalLineCount = TotalLinesRead,
                    EllapsedMillis = Elapsed.TotalMilliseconds,
                }.ToString();
    }
}
