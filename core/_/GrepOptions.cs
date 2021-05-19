using System.Text;

namespace YAGrep {
    public class GrepOptions {
        public Encoding Encoding { get; }
        public bool SilentCancel { get; }
        public int BufferSize { get; }
        public bool Trim { get; }
        public bool CaptureStatistics { get; set; }
        public int MaxCount { get; }

        public GrepOptions(
                Encoding? encoding = null, bool silentCancel = true, int bufferSize = _128Kb,
                bool trim = false, int maxCount = -1, bool captureStatistics = false) {
            Encoding = encoding ?? Encoding.UTF8;
            MaxCount = maxCount;
            SilentCancel = silentCancel;
            BufferSize = bufferSize;
            Trim = trim;
            CaptureStatistics = captureStatistics;
        }

        public static GrepOptions Default { get; } = new();

        private const int _128Kb = 128 * 1024;
    }
}
