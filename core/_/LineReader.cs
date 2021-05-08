using System.IO;
using System.Threading.Tasks;

namespace YAGrep {
    public class LineReader {
        private readonly StreamReader _haystack;
        private readonly Line _lineRef = new();

        private char[] _buffer;

        public LineReader(StreamReader haystack, GrepOptions options) {
            _buffer = new char[options.BufferSize];
            _haystack = haystack;
        }

        public async Task<Line> ReadLine() {
            var (lineLimit, readCount) = _lineRef.NextLine();

            while (lineLimit == -1) {
                readCount += await _haystack.ReadAsync(_buffer, readCount, _buffer.Length - readCount);
                if (readCount == 0) return _lineRef;
                _lineRef.StartChunk(_buffer, readCount);
                (lineLimit, readCount) = _lineRef.NextLine();
                if (lineLimit != -1) break;

                // expanding buffer
                var currentBuffer = _buffer;
                _buffer = new char[_buffer.Length * 2];
                currentBuffer.CopyTo(_buffer, 0);
            }

            return _lineRef;
        }

    }
}
