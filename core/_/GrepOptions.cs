using System.Text;

namespace YAGrep {
    public class GrepOptions {
        public Encoding Encoding { get; }
        public bool SilentCancel { get; }

        public GrepOptions(Encoding? encoding = null, bool? silentCancel = null) {
            Encoding = encoding ?? Encoding.UTF8;
            SilentCancel = silentCancel ?? true;
        }

        public static GrepOptions Default { get; } = new();
    }
}
