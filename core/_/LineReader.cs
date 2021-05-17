using System;
using System.IO;
using System.Threading.Tasks;

namespace YAGrep {
    public class LineReader {
        private readonly StreamReader _haystack;

        private char[] _buffer;
        private int _notReadCount = -1;
        private int _writeOnBufferStart;
        private bool _eof;

        private int _lineStartPosition = -1;
        private int _lineEndPosition = -1;
        private GrepOptions _options;

        public LineReader(StreamReader haystack, GrepOptions options) {
            _buffer = new char[options.BufferSize];
            _haystack = haystack;
            _options = options;
        }

        public async Task<Line> NextLine() {
            if (_eof && !ThereIsMoreToReadFromBuffer()) return Line.EndOfFile;

            (_lineStartPosition, _lineEndPosition, _notReadCount) =
                _notReadCount == -1
                    ? (0, -1, 0)
                    : (_lineStartPosition, FindLineLimit(), _notReadCount);

            while (_lineEndPosition == -1)  {
                var (currentRead, streamIsOver) = _eof
                    ? (0, true)
                    : await ReadFromStream(firstRead: _notReadCount == -1);
                _notReadCount += currentRead;
                _eof = streamIsOver;

                if (_eof && _notReadCount == 0) return Line.EndOfFile;

                _lineEndPosition = FindLineLimit();
                if(_lineEndPosition > -1) break;
                // could not find line break
                if (_eof) {
                    var lastLine = new Line(_buffer, _lineStartPosition, _notReadCount);
                    _lineStartPosition += lastLine.Length;
                    _notReadCount = 0;
                    return lastLine;
                }

                // more to read, but not line end yet, so we must expand the buffer
                var currentBuffer = _buffer;
                _buffer = new char[_buffer.Length * 2];
                currentBuffer.CopyTo(_buffer, 0);
            }

            var lineLength = _lineEndPosition - _lineStartPosition;
            var currentLine = new Line(_buffer, _lineStartPosition, lineLength);
            _lineStartPosition = _lineEndPosition + 1; // preparing next line
            _notReadCount -= (lineLength + 1); // marking as read on the buffer
            return currentLine;
        }

        private bool ThereIsMoreToReadFromBuffer() => _notReadCount > 0;

        private int FindLineLimit() {
            if (_notReadCount < 0) return -1; // never touched the stream

            for (var i = _lineStartPosition; i < _buffer.Length; i++)
                if (_buffer[i] == '\n') return i;
            return -1;
        }

        private async Task<(int currentRead, bool isEof)> ReadFromStream(bool firstRead) {
            // there are left overs from the last read?
            if (!firstRead && _lineStartPosition > 0 && _lineEndPosition == -1) {
                Array.Copy(_buffer, _lineStartPosition, _buffer, 0, _notReadCount); // shifting left overs to the start
                _writeOnBufferStart = _notReadCount;
                _lineStartPosition = 0;
            }

            var spaceAvailableOnBuffer = firstRead ? _buffer.Length : _buffer.Length - _notReadCount;

            var readCount = await _haystack.ReadAsync(_buffer, _writeOnBufferStart, spaceAvailableOnBuffer);
            _writeOnBufferStart += readCount;
            return (readCount,
                    isEof: readCount < spaceAvailableOnBuffer);
        }
    }
}
