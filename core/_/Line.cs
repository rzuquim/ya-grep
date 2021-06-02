using System;
using System.IO;
using System.Threading.Tasks;

namespace YAGrep {
    public class Line {
        private readonly char[] _buffer;
        private int _startIndex;
        private readonly bool _trim;
        public int Length { get; private set; }

        public Line(string line, bool trim = true) : this(line.ToCharArray(), trim) =>
            Update(startIndex: 0, line.Length);

        public Line(char[] buffer, bool trim = false) {
            _buffer = buffer;
            _trim = trim;
        }

        public Line Update(int startIndex, int length) {
            _startIndex = startIndex;
            Length =
                length == 0 ? length : // empty line
                length < 0 || startIndex + length > _buffer.Length ? - 1 : // is valid?
                _buffer[_startIndex + length - 1] == '\r' ? length - 1 : // windows \r\n
                length;

            if (_trim)
                (_startIndex, Length) = SearchForNonBlankLimits(_startIndex, Length);
            return this;
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

        public static readonly Line EndOfFile = new("") { Length = -1 };

        public async Task FlushInto(StreamWriter target, bool autoFlush) {
            await target.WriteLineAsync(_buffer, _startIndex, Length);
            if (autoFlush) await target.FlushAsync();
        }

        // Private
        private (int startIndex, int length) SearchForNonBlankLimits(int startIndex, int length) {
            var startSpacesCount = 0;
            for (var i = 0; i < length; i++) {
                if (_buffer[_startIndex + i] == ' ') startSpacesCount++;
                else break;
            }

            // only spaces in line
            if (startSpacesCount >= length) return (0, 0);

            var endSpacesCount = 0;
            for (var i = 0; i < length; i++) {
                if (_buffer[_startIndex + length - i - 1] == ' ') endSpacesCount++;
                else break;
            }

            return (
                startIndex: startIndex + startSpacesCount,
                length: length - (startSpacesCount + endSpacesCount)
            );
        }
    }
}
