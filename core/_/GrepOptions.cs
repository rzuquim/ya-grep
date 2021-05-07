using System.Text;

namespace YAGrep {
    public class GrepOptions {
        public Encoding Encoding { get; }
        public bool SilentCancel { get; }
        public int BufferSize { get; }

        public GrepOptions(Encoding? encoding = null, bool? silentCancel = null, int? bufferSize = null) {
            Encoding = encoding ?? Encoding.UTF8;
            SilentCancel = silentCancel ?? true;
            BufferSize = bufferSize ?? _2Kb;
        }

        public static GrepOptions Default { get; } = new();

        private const int _2Kb = 2 * 1024;
    }
}
