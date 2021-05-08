using System;
using System.Collections.Generic;

namespace YAGrep {
    public class Line {
        private char[] _buffer;
        private int _lineIndex = -1;
        private int _lineLimit = -1;
        private int _readCount;

        public int Length => _lineLimit - _lineIndex;
        public int this[int i] => _buffer[_lineIndex + i];

        public Line() { }

        public Line(string line) {
            _buffer = line.ToCharArray();
            _lineIndex = 0;
            _readCount = line.Length;
            _lineLimit = line.Length;
        }

        public void StartChunk(char[] buffer, int readCount) {
            _buffer = buffer;
            _lineIndex = 0;
            _readCount = readCount;
            _lineLimit = 0;
        }

        public (int lineLimit, int readCount) NextLine() {
            if (_readCount == 0) return (-1, _readCount);
            var nextLineIndex = _lineLimit > 0 ? _lineLimit + 1 : 0;
            (_lineIndex, _lineLimit) = (nextLineIndex, FindLineEnd(_buffer, nextLineIndex, _readCount));

            if (_lineLimit != -1) return (_lineLimit, _readCount);

            _readCount -= nextLineIndex;
            if (_readCount == 0) return (-1, _readCount);
            Array.Copy(_buffer, nextLineIndex, _buffer, 0, _readCount); // shifting left overs
            return (-1, _readCount);
        }

        public int IndexOf(char needle, int startIndex, int limit) =>
            IndexOf(_buffer, needle, _lineIndex + startIndex, limit) - _lineIndex;

        public bool Valid() => _lineIndex < _lineLimit;

        public Line Clone() => new(AsString());

        public string AsString() => new(_buffer, _lineIndex, _lineLimit - _lineIndex);

        private static int FindLineEnd(IReadOnlyList<char> buffer, int currentIndex, int readCount) {
            var lineLimit = IndexOf(buffer, needle: '\n', currentIndex, readCount - currentIndex);
            return lineLimit > 0 ? lineLimit :
                   currentIndex > readCount ? -1 :
                   readCount;
        }

        private static int IndexOf(IReadOnlyList<char> haystack, char needle, int startIndex, int count) {
            var limit = startIndex + count;
            for (var i = startIndex; i < limit; i++)
                if (haystack[i] == needle) return i;
            return -1;
        }

        private static readonly char[] _empty = new char[0];
    }
}
