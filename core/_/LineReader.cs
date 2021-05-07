using System;
using System.IO;
using System.Threading.Tasks;

namespace YAGrep {
    public class LineReader {
        private readonly StreamReader _haystack;
        private readonly Line _lineRef = new();

        private char[] _buffer;
        private int _lineIndex = 0;

        public LineReader(StreamReader haystack, GrepOptions options) {
            _buffer = new char[options.BufferSize];
            _haystack = haystack;
        }

        public async Task<Line> ReadLine() {
            var (lineLimit, readCount) = _lineRef.NextLine(_lineIndex);

            while (lineLimit == -1) {
                readCount += await _haystack.ReadAsync(_buffer, readCount, _buffer.Length - readCount);
                if (readCount == 0) return _lineRef;
                lineLimit = _lineRef.StartChunk(_buffer, readCount);
                if (lineLimit != -1) break;

                // expanding buffer
                var currentBuffer = _buffer;
                _buffer = new char[_buffer.Length * 2];
                currentBuffer.CopyTo(_buffer, 0);
            }

            _lineIndex = lineLimit + 1;
            return _lineRef;
        }

        private static readonly char[] _empty = new char[0];
    }

    public class Line {
        private char[] _buffer;
        private int _lineIndex = -1;
        private int _lineLimit = -1;
        private int _readCount;

        public int StartChunk(char[] buffer, int readCount) {
            _buffer = buffer;
            _lineIndex = 0;
            _readCount = readCount;
            _lineLimit = FindNextLine(_buffer, _lineIndex, _readCount);
            return _lineLimit;
        }

        public (int lineLimit, int readCount) NextLine(int lineIndex) {
            if (_readCount == 0)  return (-1, _readCount);
            (_lineIndex, _lineLimit) = (lineIndex, FindNextLine(_buffer, lineIndex, _readCount));

            if (_lineLimit != -1) return (_lineLimit, _readCount);

            _readCount -= lineIndex;
            if (_readCount == 0) return (-1, _readCount);
            Array.Copy(_buffer, lineIndex, _buffer, 0, _readCount); // shifting left overs
            return (-1, _readCount);
        }

        public bool Valid() => _lineIndex < _lineLimit;

        private static int FindNextLine(char[] buffer, int currentIndex, int readCount) =>
            buffer.IndexOf(needle: '\n', currentIndex, readCount - currentIndex);

        public override string ToString() => new(_buffer, _lineIndex, _lineLimit - _lineIndex);
    }
}
