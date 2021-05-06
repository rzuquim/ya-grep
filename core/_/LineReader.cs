using System.IO;
using System.Threading.Tasks;

namespace YAGrep {
    public class LineReader {
        private readonly StreamReader _haystack;

        public LineReader(StreamReader haystack) => _haystack = haystack;

        public async Task<char[]> TryReadLine() {
            var line = await _haystack.ReadLineAsync();
            return line == null ? _empty : line.ToCharArray();
        }

        private static readonly char[] _empty = new char[0] ;
    }
}
