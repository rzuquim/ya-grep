using System;

namespace YAGrep {
    public readonly struct Line {
        private readonly char[] _buffer;
        private readonly int _startIndex;
        private readonly bool _trim;
        public int Length { get; }

        public Line(string line, bool trim = true) : this(line.ToCharArray(), 0, line.Length, trim) { }

        public Line(char[] buffer, int startIndex, int length, bool trim = true) {
            _buffer = buffer;
            _startIndex = startIndex;
            _trim = trim;
            Length =
                length == 0 ? length : // empty line
                length < 0 || startIndex + length > _buffer.Length ? - 1 : // is valid?
                _buffer[_startIndex + length - 1] == '\r' ? length - 1 : // windows \r\n
                length;
        }

        public int this[int i] => _buffer[_startIndex + i];

        public Line Snapshot() => new(AsString());

        public int IndexOf(char needle, int startSearch, int limit) {
            if (!Valid() || startSearch < 0) return -1;

            limit = Math.Min(
                limit + startSearch + _startIndex,
                _startIndex + Length); // enforcing line boundaries

            for (var i = _startIndex + startSearch; i < limit; i++)
                if (_buffer[i] == needle) return i - _startIndex;

            return -1;
        }

        public bool Valid() => Length >= 0;

        public string AsString() => new(_buffer, _startIndex, Length);

        public static readonly Line EndOfFile = new(new char[0], startIndex: 0, length: -1);
    }
}
